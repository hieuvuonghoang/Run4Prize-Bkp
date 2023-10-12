using com.strava.v3.api.Authentication;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Services;

namespace Run4Prize.Jobs
{
    public class JobRefreshToken : IJob
    {
        public static JobKey jobKey = new JobKey(nameof(JobRefreshToken), "groupOne");
        public static string JobParamKey = "JobParmaKey";

        private readonly ILogger<JobSyncActivites> _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobRefreshToken(ILogger<JobSyncActivites> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                try
                {
                    var date = DateTime.UtcNow.AddHours(1);
                    var tokenNeedUpdates = await dbContext.Tokens.Where(it => it.utcexpireat <= date)
                        .ToListAsync();
                    var tokenNeedRemoves = await dbContext.Tokens.Where(it => it.utcexpireat < DateTime.UtcNow)
                        .ToListAsync();
                    foreach (var item in tokenNeedRemoves)
                    {
                        dbContext.Tokens.Entry(item).State = EntityState.Deleted;
                    }
                    var stravaServices = scope.ServiceProvider.GetRequiredService<IStravaServices>();
                    foreach (var item in tokenNeedUpdates)
                    {
                        try
                        {
                            var aT = new AccessToken()
                            {
                                Token = item.token,
                                RefreshToken = item.refreshtoken,
                            };
                            var accessToken = await stravaServices.RefreshToken(aT);
                            item.EntityUpdateDate = DateTime.UtcNow;
                            item.token = accessToken.Token;
                            item.refreshtoken = accessToken.RefreshToken;
                            item.expiresin = accessToken.ExpiresIn;
                            item.expiresat = accessToken.ExpiresAt;
                            item.utcexpireat = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(accessToken.ExpiresAt));
                            dbContext.Tokens.Entry(item).State = EntityState.Modified;
                        } catch(Exception ex)
                        {
                            dbContext.Logs.Add(new LogEntity()
                            {
                                Message = $"JobName: {nameof(JobRefreshToken)} - Tokens ID: {item.Id} - ERR: {ex.Message}",
                                EntityCreateDate = DateTime.UtcNow,
                                EntityUpdateDate = DateTime.UtcNow,
                                Type = "ERR"
                            });
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    await dbContext.SaveChangesAsync();
                } catch(Exception ex)
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
