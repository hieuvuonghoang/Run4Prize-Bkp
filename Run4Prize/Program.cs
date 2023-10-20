using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;
using Run4Prize.AutoMapper;
using Run4Prize.Jobs;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains;
using Run4Prize.Services;
using Run4Prize.Services.AccessTokenServices;
using Run4Prize.Services.ActivityServices;
using Run4Prize.Services.AthleteServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<Run4PrizeAppConfig>(builder.Configuration.GetSection(nameof(Run4PrizeAppConfig)));

builder.Services.AddHttpClient(StravaServices.URL_GET_ACCESS_TOKEN, httpClient =>
{
    httpClient.BaseAddress = new Uri(StravaServices.URL_GET_ACCESS_TOKEN);
});

builder.Services.AddHttpClient(StravaServices.URL_GET_ACTIVITIES, httpClient =>
{
    httpClient.BaseAddress = new Uri(StravaServices.URL_GET_ACTIVITIES);
});

builder.Services.AddScoped<IStravaServices, StravaServices>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(90);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Forbidden/";
                });

builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IAthleteServices, AthleteServices>();

builder.Services.AddScoped<IAccessTokenServices, AccessTokenServices>();

builder.Services.AddScoped<IActivityServices, ActivityServices>();

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

builder.Services.AddSingleton<JobSyncActivites>();

builder.Services.AddSingleton<JobRefreshToken>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    var taskMigrate = dbContext.Database.MigrateAsync();
    taskMigrate.Wait();

    //var weeks = new List<WeekEntity>();
    //// From 00:00 ngày 21/10/2023 – 23:59 To 17/12/2023
    //var timeZone = TimeZoneInfo
    //    .GetSystemTimeZones().Where(it => it.BaseUtcOffset == TimeSpan.FromHours(7))
    //    .First();
    //weeks.Add(new WeekEntity()
    //{
    //    Name = $"Tập luyện 1",
    //    FromDate = new DateTimeWithZone(new DateTime(2023, 10, 14, 00, 00, 00), timeZone).UniversalTime,
    //    ToDate = new DateTimeWithZone(new DateTime(2023, 10, 20, 23, 59, 59), timeZone).UniversalTime,
    //    IsActive = true,
    //    DistanceTarget = null,
    //});
    //var fromDate = new DateTimeWithZone(new DateTime(2023, 10, 21, 00, 00, 00), timeZone).UniversalTime;
    //var toDate = new DateTimeWithZone(new DateTime(2023, 12, 17, 23, 59, 59), timeZone).UniversalTime;
    //weeks.Add(new WeekEntity()
    //{
    //    Name = $"Tất cả",
    //    FromDate = fromDate,
    //    ToDate = toDate,
    //    IsActive = true,
    //    DistanceTarget = 450
    //});
    //for (var i = 1; i <= 9; i++)
    //{
    //    var isDone = false;
    //    var toDateTmp = fromDate.AddDays(7).AddSeconds(-1);
    //    if (toDateTmp > toDate)
    //    {
    //        toDateTmp = toDate;
    //        isDone = false;
    //    }
    //    weeks.Add(new WeekEntity()
    //    {
    //        Name = $"Tuần {i}",
    //        FromDate = fromDate.AddSeconds(0),
    //        ToDate = toDateTmp,
    //        IsActive = true,
    //        DistanceTarget = null,
    //    });
    //    fromDate = fromDate.AddDays(7);
    //    if (isDone)
    //        break;
    //}
    //var weekEntities = dbContext.Weeks.ToList();
    //foreach(var weekEntity in weekEntities)
    //{
    //    dbContext.Entry(weekEntity).State = EntityState.Deleted;
    //}
    //dbContext.SaveChanges();
    //foreach (var week in weeks)
    //{
    //    dbContext.Weeks.Add(week);
    //}
    //dbContext.SaveChanges();

    var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
    var task = schedulerFactory.GetScheduler();
    task.Wait();
    var scheduler = task.Result;

    // define the job and tie it to our HelloJob class
    var jobSyncActivites = JobBuilder.Create<JobSyncActivites>()
        .WithIdentity(JobSyncActivites.jobKey)
        .Build();

    // Trigger the job to run now, and then every 40 seconds
    var triggerJobSyncActivites = TriggerBuilder.Create()
        .WithIdentity("myTrigger", "groupOne")
        .StartNow()
        //.WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(30)).RepeatForever())
        .WithSchedule(
            CronScheduleBuilder.CronSchedule("0 0 0 ? * * *")
            .InTimeZone(TimeZoneInfo.GetSystemTimeZones().Where(it => it.BaseUtcOffset == TimeSpan.FromHours(7))
            .First())
        )
        .Build();

    var jobRefreshToken = JobBuilder.Create<JobRefreshToken>()
        .WithIdentity(JobRefreshToken.jobKey)
        .Build();

    // Trigger the job to run now, and then every 40 seconds
    var triggerJobRefreshToken = TriggerBuilder.Create()
        .WithIdentity("triggerJobRefreshToken", "groupOne")
        .StartNow()
        //.WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(60)).RepeatForever())
        .WithSchedule(
            CronScheduleBuilder.CronSchedule("0 0,15,30,45 * ? * * *")
            .InTimeZone(TimeZoneInfo.GetSystemTimeZones().Where(it => it.BaseUtcOffset == TimeSpan.FromHours(7))
            .First())
        )
        .Build();

    var taskB = scheduler.ScheduleJob(jobSyncActivites, triggerJobSyncActivites);
    taskB.Wait();

    var taskC = scheduler.ScheduleJob(jobRefreshToken, triggerJobRefreshToken);
    taskC.Wait();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

