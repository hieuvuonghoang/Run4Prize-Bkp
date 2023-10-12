using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains;
using Run4Prize.Services;
using Run4Prize.Services.ActivityServices;

namespace Run4Prize.Jobs
{
    public class JobSyncActivites : IJob
    {
        public static JobKey jobKey = new JobKey(nameof(JobSyncActivites), "groupOne");
        public static string JobParamKey = "JobParmaKey";

        private readonly ILogger<JobSyncActivites> _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobSyncActivites(ILogger<JobSyncActivites> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using(var scope = _serviceProvider.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                try
                {
                    var isJobParam = context.MergedJobDataMap.GetBoolean(JobParamKey);
                    var jobParams = new List<JobParameter>();
                    var today = DateTime.UtcNow;
                    var firstDayOfMonth = new DateTime(today.Year, today.Month, 01);
                    if (isJobParam)
                    {
                        jobParams = await dbContext.JobParameters
                            .Where(it => it.JobName == nameof(JobSyncActivites) && it.IsExecuted == false)
                            .ToListAsync();
                    } else
                    {
                        var accessTokens = await dbContext.Tokens.Select(it => new AccessTokenEntity()
                        {
                            token = it.token
                        }).ToListAsync();
                        foreach(var token in accessTokens)
                        {
                            jobParams.Add(new JobParameter()
                            {
                                Parameter = JsonConvert.SerializeObject(new JobSyncActivityParameter()
                                {
                                    AccessToken = token.token,
                                    FromDate = firstDayOfMonth,
                                }),
                                JobName = nameof(JobSyncActivites)
                            });
                        }
                    }
                    if(jobParams != null && jobParams.Count != 0)
                    {
                        var stravaServices = scope.ServiceProvider.GetRequiredService<IStravaServices>();
                        var activityServices = scope.ServiceProvider.GetRequiredService<IActivityServices>();
                        foreach(var jobParam in jobParams)
                        {
                            try
                            {
                                jobParam.ExecuteStartDate = DateTime.UtcNow;
                                var objJobParam = JsonConvert.DeserializeObject<JobSyncActivityParameter>(jobParam.Parameter!);
                                objJobParam!.FromDate = objJobParam.FromDate ?? firstDayOfMonth;
                                var activites = await stravaServices.GetActivities(objJobParam!.AccessToken!, objJobParam.FromDate.Value);
                                var activitiEntities = await activityServices.AddIfNoExists(activites);
                                jobParam.IsExecuted = true;
                                jobParam.ExecuteEndDate = DateTime.UtcNow;
                                if(jobParam.Id == 0)
                                    await dbContext.JobParameters.AddAsync(jobParam);
                                dbContext.SaveChanges();
                            } catch(Exception ex)
                            {
                                jobParam.IsError = true;
                                dbContext.Logs.Add(new LogEntity()
                                {
                                    Message = $"JobName: {nameof(JobSyncActivites)} - JobParameters ID:  {jobParam.Id} - ERR: {ex.Message}",
                                    EntityCreateDate = DateTime.UtcNow,
                                    EntityUpdateDate = DateTime.UtcNow,
                                    Type = "ERR"
                                });
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContext.Logs.Add(new LogEntity()
                    {
                        Message = ex.Message,
                        EntityCreateDate = DateTime.UtcNow,
                        EntityUpdateDate = DateTime.UtcNow,
                        Type = "ERR"
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
