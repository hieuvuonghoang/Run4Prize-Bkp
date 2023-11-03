using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Run4Prize.Enum;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains.Actitvity;
using Run4Prize.Models.Domains.ScoreBoard;
using Run4Prize.Services;

namespace Run4Prize.Jobs
{
    public class JobSyncData : IJob
    {
        public static JobKey jobKey = new JobKey(nameof(JobSyncData), "groupOne");

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobSyncData(ILogger<JobSyncData> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                try
                {
                    var topTeamService = scope.ServiceProvider.GetRequiredService<ITopTeamServices>();
                    var scoreBoardServices = scope.ServiceProvider.GetRequiredService<IScoreBoardServices>();
                    var activityServices = scope.ServiceProvider.GetRequiredService<IActivityServices>();

                    var topTeamData = await topTeamService.GetDatas();

                    var teamId = await dbContext.Settings.Where(it => it.Type == (int)EnumSetting.TeamId).FirstOrDefaultAsync();
                    if (teamId == null || string.IsNullOrEmpty(teamId.Value))
                    {
                        throw new Exception("Setting teamId or teamId value is null.");
                    }

                    var findTeam = topTeamData.data!.result!.Where(it => it.teamId == teamId.Value)
                        .FirstOrDefault();

                    if (findTeam == null)
                    {
                        throw new Exception($"Not found teamId = {teamId.Value}");
                    }

                    var numTeam = await dbContext.Settings.Where(it => it.Type == (int)EnumSetting.NumTeam).FirstOrDefaultAsync();
                    var numTeamValue = 2;
                    if (numTeam != null && !string.IsNullOrEmpty(numTeam.Value))
                    {
                        if (!int.TryParse(numTeam.Value, out numTeamValue))
                        {
                            // default
                            numTeamValue = 2;
                        }
                    }
                    topTeamData.data!.result = topTeamData.data!.result!.OrderBy(it => it.rank).ToList();

                    var indexTeam = topTeamData.data!.result!.IndexOf(findTeam);
                    var teams = new List<Models.Domains.TopTeam.Result>()
                    {
                        findTeam
                    };
                    var skipA = 0;
                    var skipB = 0;
                    var takeA = 0;
                    var takeB = 0;
                    double rank = 0;
                    var takeTmp = 0;

                    #region ""
                    rank = findTeam.rank;
                    takeTmp = 0;
                    while (true)
                    {
                        if (rank - 1 <= 0 || takeTmp == numTeamValue)
                        {
                            break;
                        }
                        ++takeTmp;
                        --rank;
                    }
                    takeA = takeTmp;
                    skipA = indexTeam - takeA;
                    #endregion

                    #region ""
                    rank = findTeam.rank;
                    takeTmp = 0;
                    while (true)
                    {
                        if (rank + 1 >= topTeamData.data!.total || takeTmp == numTeamValue)
                        {
                            break;
                        }
                        ++takeTmp;
                        ++rank;
                    }
                    takeB = takeTmp;
                    skipB = indexTeam + 1;
                    #endregion

                    teams.AddRange(topTeamData.data!.result!.Skip(skipA).Take(takeA).ToList());
                    teams.AddRange(topTeamData.data!.result!.Skip(skipB).Take(takeB).ToList());

                    teams = teams.OrderBy(it => it.rank).ToList();
                    var taskGetScoreBoardData = new List<Task<ScroeBoardBodyResponse>>();
                    foreach (var team in teams)
                    {
                        var uId = team.teamId;
                        taskGetScoreBoardData.Add(scoreBoardServices.GetDatas(uId!));
                    }
                    var taskGetScoreBoardDataResult = await Task.WhenAll(taskGetScoreBoardData);
                    var taskGetActivityData = new List<Task<ActivityBodyResponse>>();
                    foreach (var taskGetScoreBoardDataResultTmp in taskGetScoreBoardDataResult)
                    {
                        foreach (var item in taskGetScoreBoardDataResultTmp.data!.result!)
                        {
                            var uId = item.uId;
                            taskGetActivityData.Add(activityServices.GetDatas(uId!));
                        }
                    }
                    var taskGetActivityDataResult = await Task.WhenAll(taskGetActivityData);

                    var scoreBoardResults = new List<Models.Domains.ScoreBoard.Result>();
                    foreach (var item in taskGetScoreBoardDataResult)
                    {
                        scoreBoardResults.AddRange(item.data!.result!);
                    }

                    var activityDataResults = new List<Datum>();
                    foreach(var item in taskGetActivityDataResult)
                    {
                        activityDataResults.AddRange(item.data!);
                    }

                    #region "Delete Data"
                    using(var tran = dbContext.Database.BeginTransaction())
                    {
                        var tableTeam = await dbContext.Teams.ToListAsync();
                        foreach (var team in tableTeam)
                        {
                            dbContext.Entry(team).State = EntityState.Deleted;
                        }
                        var tableMember = await dbContext.Members.ToListAsync();
                        foreach (var member in tableMember)
                        {
                            dbContext.Entry(member).State = EntityState.Deleted;
                        }
                        var tableDistance = await dbContext.Distances.ToListAsync();
                        foreach (var distance in tableDistance)
                        {
                            dbContext.Entry(distance).State = EntityState.Deleted;
                        }
                        await dbContext.SaveChangesAsync();
                        await tran.CommitAsync();
                    }
                    
                    #endregion

                    var iA = await dbContext.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT('Teams', RESEED, 0);");
                    var iB = await dbContext.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT('Members', RESEED, 0);");
                    var iC = await dbContext.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT('Distances', RESEED, 0);");

                    #region "Insert Data"
                    using (var tran = dbContext.Database.BeginTransaction())
                    {
                        foreach (var team in teams)
                        {
                            var teamEntity = new Team()
                            {
                                Name = team.name,
                                Rank = team.rank,
                                Uid = team.teamId
                            };
                            dbContext.Teams.Add(teamEntity);
                            dbContext.SaveChanges();
                            var scoreBoards = scoreBoardResults.Where(it => it.teamId == team.teamId).ToList();
                            var memberEntitys = new List<Models.DBContexts.AppContext.Member>();
                            foreach(var scoreBoard in scoreBoards)
                            {
                                var memberEntity = new Models.DBContexts.AppContext.Member()
                                {
                                    TeamId = teamEntity.Id,
                                    Name = scoreBoard.name,
                                };
                                dbContext.Members.Add(memberEntity);
                                dbContext.SaveChanges();
                                var activities = activityDataResults.Where(it => it.user == scoreBoard.uId).ToList();
                                var distances = new List<Models.DBContexts.AppContext.Distance>();
                                foreach(var activity in activities)
                                {
                                    activity.createdDate = activity.createdDate.AddHours(7);
                                    double distance = 0;
                                    if(activity.type == "Run" || activity.type == "Walk")
                                    {
                                        distance = activity.distance / 1000;
                                    }
                                    else if(activity.type == "Bike")
                                    {
                                        distance = activity.distance / 3000;
                                    }
                                    var findByDate = distances.Where(it => it.CreateDate.Year == activity.createdDate.Year &&
                                        it.CreateDate.Month == activity.createdDate.Month &&
                                        it.CreateDate.Day == activity.createdDate.Day).FirstOrDefault();
                                    if (findByDate != null)
                                    {
                                        findByDate.TotalDistance += distance;
                                    } else
                                    {
                                        findByDate = new Models.DBContexts.AppContext.Distance()
                                        {
                                            CreateDate = new DateTime(activity.createdDate.Year, activity.createdDate.Month, activity.createdDate.Day),
                                            MemberId = memberEntity.Id,
                                            TotalDistance = distance
                                        };
                                        distances.Add(findByDate);
                                    }
                                }
                                dbContext.Distances.AddRange(distances);
                                dbContext.SaveChanges();
                            }
                        }
                        await tran.CommitAsync();
                    }

                    #endregion

                    dbContext.Logs.Add(new Log()
                    {
                        Mess = "JobSyncData Done!",
                        Type = "INF"
                    });
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    dbContext.Logs.Add(new Log()
                    {
                        Mess = ex.Message,
                        Type = "ERR"
                    });
                    await dbContext.SaveChangesAsync();
                }

            }
        }
    }
}
