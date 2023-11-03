using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;
using Run4Prize.AutoMapper;
using Run4Prize.Enum;
using Run4Prize.Jobs;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains.Configs;
using Run4Prize.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<APIConfig>(builder.Configuration.GetSection(nameof(APIConfig)));

builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddHttpClient(nameof(APIConfig), cfg =>
{
    cfg.BaseAddress = new Uri(builder.Configuration.GetSection(nameof(APIConfig)).GetSection(nameof(APIConfig.DomainUrl)).Value!);
});

builder.Services.AddQuartz(q =>
{
    // base Quartz scheduler, job and trigger configuration
});

// ASP.NET Core hosting
builder.Services.AddQuartzServer(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

builder.Services.AddScoped<ITopTeamServices, TopTeamServices>();
builder.Services.AddScoped<IScoreBoardServices, ScoreBoardServices>();
builder.Services.AddScoped<IActivityServices, ActivityServices>();

builder.Services.AddSingleton<JobSyncData>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    var taskMigrate = dbContext.Database.MigrateAsync();
    taskMigrate.Wait();

    #region "Seeds DataBase"

    var settingCookie = dbContext.Settings.Where(it => it.Type == (int)EnumSetting.Cookie).FirstOrDefault();
    if(settingCookie == null)
    {
        dbContext.Settings.Add(new Setting()
        {
            Type = (int)EnumSetting.Cookie,
            Value = "locale=vi; " +
                    "vr-auth_org=s%3AeyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2NTFhODgwMzI2NTk2YTI5YjM4ZGEzNmYiLCJpYXQiOjE2OTg3MTk1NDcsImV4cCI6MTcwMTMxMTU0N30.PQ_IAj3ZMozBEFj3Tzr2Qrj8zatLkiFlYmLYSntifJ7DY6Yc_QQPoMyc1n7XNGkgzLDF2WGJgfh5N_wWS2oy5g.lkrInIP8zLPWyyJ4gbHqg01UuMTZUex4MO3TLs%2FzzmU; " +
                    "connect.sid=s%3ADxiz9kA3vFlWgvz0QROcu6qSlmNUhv4W.U2ywOdImY4w9bgdSSwHm3tGkmdP2VdsHII4F38vrFIA"
        });
        dbContext.SaveChanges();
    }

    var settingTeamId = dbContext.Settings.Where(it => it.Type == (int)EnumSetting.TeamId).FirstOrDefault();
    if (settingTeamId == null)
    {
        dbContext.Settings.Add(new Setting()
        {
            Type = (int)EnumSetting.TeamId,
            Value = "651bf055e17b027583a90fee"
        });
        dbContext.SaveChanges();
    }

    var settingNumTeam = dbContext.Settings.Where(it => it.Type == (int)EnumSetting.NumTeam).FirstOrDefault();
    if (settingNumTeam == null)
    {
        dbContext.Settings.Add(new Setting()
        {
            Type = (int)EnumSetting.NumTeam,
            Value = "2"
        });
        dbContext.SaveChanges();
    }

    #endregion

    var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
    var taskSchedulerFactory = schedulerFactory.GetScheduler();
    taskSchedulerFactory.Wait();
    var scheduler = taskSchedulerFactory.Result;

    var jobSyncData = JobBuilder.Create<JobSyncData>()
        .WithIdentity(JobSyncData.jobKey)
        .Build();

    // Trigger the job to run now, and then every 40 seconds
    var triggerJobSyncData = TriggerBuilder.Create()
        .WithIdentity("myTrigger", "groupOne")
        .StartNow()
        //.WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMinutes(10)).RepeatForever())
        .WithSchedule(
            CronScheduleBuilder.CronSchedule("0 0,10,20,30,40,50 5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23 ? * * *")
            .InTimeZone(TimeZoneInfo.GetSystemTimeZones().Where(it => it.BaseUtcOffset == TimeSpan.FromHours(7))
            .First())
        )
        .Build();

    var taskSchedulerJobSyncData = scheduler.ScheduleJob(jobSyncData, triggerJobSyncData);
    taskSchedulerJobSyncData.Wait();

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

