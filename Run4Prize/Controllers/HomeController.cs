using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Run4Prize.Models;
using Run4Prize.Services;
using Run4Prize.Services.AthleteServices;
using Run4Prize.Services.AccessTokenServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Run4Prize.Models.Domains;
using Microsoft.Extensions.Options;
using Run4Prize.Constants;
using Run4Prize.Services.ActivityServices;
using Run4Prize.Models.DBContexts.AppContext;
using Microsoft.EntityFrameworkCore;
using Run4Prize.Jobs;
using Quartz;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using Run4Prize.Filters;

namespace Run4Prize.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStravaServices _stravaServices;
        private readonly IAthleteServices _athleteServices;
        private readonly IAccessTokenServices _accessTokenServices;
        private readonly Run4PrizeAppConfig _run4PrizeAppConfig;
        private readonly IActivityServices _activityServices;
        private readonly AppDBContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,
            IStravaServices stravaServices,
            IAthleteServices athleteServices,
            IAccessTokenServices accessTokenServices,
            IOptions<Run4PrizeAppConfig> run4PrizeAppConfig,
            IActivityServices activityServices,
            AppDBContext dbContext,
            IServiceProvider serviceProvider,
            IMapper mapper)
        {
            _logger = logger;
            _stravaServices = stravaServices;
            _athleteServices = athleteServices;
            _accessTokenServices = accessTokenServices;
            _run4PrizeAppConfig = run4PrizeAppConfig.Value;
            _activityServices = activityServices;
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string week)
        {
            using (var scopeA = _serviceProvider.CreateScope())
            {
                var schedulerFactory = scopeA.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                var jobDataMap = new JobDataMap();
                jobDataMap.Put(JobSyncActivites.JobParamKey, false);
                await scheduler.TriggerJob(JobSyncActivites.jobKey, jobDataMap);
            }
            if (!User.Identity!.IsAuthenticated)
            {
                ViewData[ConstantDomains.UrlCallBackStrava] = $"https://www.strava.com/oauth/authorize?client_id={_run4PrizeAppConfig.client_id}&response_type={_run4PrizeAppConfig.response_type}&redirect_uri={_run4PrizeAppConfig.redirect_uri}&approval_prompt=force&scope=read%2Cactivity%3Aread%2Cactivity%3Aread_all";
            }
            else
            {
                var claims = HttpContext.User.Claims;
                var claimAvatar = claims
                    .FirstOrDefault(x => x.Type == "Avatar")?.Value;
                ViewData[ConstantDomains.UrlUserAvatar] = claimAvatar;
            }
            var weekEntitys = await _dbContext
                .Weeks
                .Where(it => it.IsActive).AsNoTracking().ToListAsync();
            weekEntitys = weekEntitys.OrderBy(it => it.Id).ToList();
            var sLWeek = new SelectList(weekEntitys, "Id", "Name");
            var weekChoose = weekEntitys.FirstOrDefault();
            var outWeek = 0;
            if (!string.IsNullOrEmpty(week) && int.TryParse(week, out outWeek))
            {
                weekChoose = weekEntitys.Where(it => it.Id == outWeek)
                    .FirstOrDefault();
            }
            if (weekChoose == null)
                return RedirectToAction(nameof(Error), new {error = "Tables Weeks empty!"});
            var query =
                from athlete in _dbContext.Athletes
                from activite in _dbContext.Activities
                where athlete.Id == activite.AthleteId && activite.StartDate > weekChoose.FromDate && activite.StartDate < weekChoose.ToDate
                select new
                {
                    athlete = athlete,
                    activite = activite
                };
            var resultQuery = query.ToList();
            var groupByAthletes = resultQuery.GroupBy(it => it.athlete).ToList();
            var datas = new List<AthleteDomain>();
            foreach (var groupByAthlete in groupByAthletes)
            {
                var athlete = _mapper.Map<AthleteEntity, AthleteDomain>(groupByAthlete.Key);
                athlete.Activities = new List<ActivityDomain>();
                foreach (var activite in groupByAthlete.ToList())
                {
                    var tmp = _mapper.Map<ActivityEntity, ActivityDomain>(activite.activite);
                    tmp.Year = activite.activite.StartDate.Year;
                    tmp.Month = activite.activite.StartDate.Month;
                    tmp.Day = activite.activite.StartDate.Day;
                    athlete.Activities.Add(tmp);
                }
                datas.Add(athlete);
            }

            var dayMonths = new List<string>();
            var i = 0;
            var timeZone = TimeZoneInfo
                .GetSystemTimeZones().Where(it => it.BaseUtcOffset == TimeSpan.FromHours(0))
                .First();
            var toDateTmp = new DateTimeWithZone(weekChoose.ToDate, timeZone).UniversalTime.AddHours(7);
            var dates = new List<DateTime>();
            while (true)
            {
                var dateTmp = new DateTimeWithZone(weekChoose.FromDate, timeZone).UniversalTime.AddHours(7).AddDays(i);
                if (dateTmp > toDateTmp)
                    break;
                dayMonths.Add(dateTmp.ToString("dd/MM"));
                dates.Add(dateTmp);
                i++;
            }
            var selected = sLWeek
                .Where(x => x.Value == weekChoose.Id.ToString())
                .First();
            selected.Selected = true;
            var chartDatasetDomains = new List<ChartDatasetDomain>();
            var athletes = _dbContext.Athletes
                .AsNoTracking()
                .ToList();
            var dataOnes = new List<AthleteDomain>();
            var weekUserDistances = await _dbContext.WeekUserDistances
                .Where(it => it.WeekId == weekChoose.Id)
                .AsNoTracking()
                .ToListAsync();
            foreach (var athlete in athletes)
            {
                var chartDatasetDomain = new ChartDatasetDomain();
                chartDatasetDomain.Data = new List<float>();
                chartDatasetDomain.Tension = 0.3f;
                chartDatasetDomain.BorderColor = athlete.ColorCode;
                chartDatasetDomain.Label = athlete.FirstName + " " + athlete.LastName;
                chartDatasetDomain.BorderWidth = 1;
                var findOne = datas.Where(it => it.Id == athlete.Id).FirstOrDefault();
                foreach (var date in dates)
                {
                    float data = 0;
                    if (findOne != null)
                    {
                        var findTwos = findOne.Activities?
                            .Where(it => it.Year == date.Year && it.Month == date.Month && it.Day == date.Day)
                            .ToList();
                        if (findTwos != null)
                        {
                            data = findTwos.Sum(it => it.Distance) / 1000;
                        }
                    }
                    chartDatasetDomain.Data.Add(data);
                }
                chartDatasetDomains.Add(chartDatasetDomain);
                var tmp = _mapper.Map<AthleteEntity, AthleteDomain>(athlete);
                if (findOne != null)
                    tmp.Activities = findOne.Activities;
                tmp.DistanceOfWeek = 0;
                var weekUserDistance = weekUserDistances.Where(it => it.AthleteId == athlete.Id).FirstOrDefault();
                if (weekUserDistance != null)
                    tmp.DistanceOfWeek = weekUserDistance.Distance;
                dataOnes.Add(tmp);
            }
            dataOnes = dataOnes.OrderBy(it => it.TotalDistance)
                .ToList();
            ViewData["dayMonths"] = JsonConvert.SerializeObject(dayMonths);
            ViewData["DataOne"] = dataOnes;
            ViewData["chartDatasets"] = JsonConvert.SerializeObject(chartDatasetDomains);
            ViewData["WeekId"] = weekChoose.Id.ToString();
            return View(sLWeek);
        }

        [AllowAnonymous]
        [TypeFilter(typeof(CustomAuthorizationFilterAttribute))]
        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [TypeFilter(typeof(CustomAuthorizationFilterAttribute))]
        public async Task<IActionResult> SyncActivityAsync()
        {
            using (var scopeA = _serviceProvider.CreateScope())
            {
                var schedulerFactory = scopeA.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                var jobDataMap = new JobDataMap();
                jobDataMap.Put(JobSyncActivites.JobParamKey, false);
                await scheduler.TriggerJob(JobSyncActivites.jobKey, jobDataMap);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [TypeFilter(typeof(CustomAuthorizationFilterAttribute))]
        public async Task<IActionResult> Distance(WeekUserDistanceDomain model)
        {
            var claims = HttpContext.User.Claims;
            var userId = claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (model.AthleteId == 0 || model.WeekId == 0 || userId != model.AthleteId.ToString())
            {
                return RedirectToAction(nameof(Error), new { error = "UserId not equals!" });
            }
            var weekExist = await _dbContext
                .Weeks
                .Where(it => it.Id == model.WeekId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (weekExist == null)
            {
                return RedirectToAction(nameof(Error), new { error = "WeekId not exist!" });
            }
            var weekUserDistances = _dbContext
                .WeekUserDistances
                .Where(it => it.AthleteId == model.AthleteId && it.WeekId == model.WeekId)
                .FirstOrDefault();
            if (weekUserDistances == null)
            {
                await _dbContext.WeekUserDistances.AddAsync(_mapper.Map<WeekUserDistanceDomain, WeekUserDistanceEntity>(model));
            }
            else
            {
                weekUserDistances.Distance = model.Distance;
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { week = model.WeekId });
        }

        [TypeFilter(typeof(CustomAuthorizationFilterAttribute))]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("exchange_token")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> StarvaCallBack(string state, string code, string scope)
        {
            if (string.IsNullOrEmpty(code))
                return RedirectToAction(nameof(Error));
            var accessToken = await _stravaServices.Authentication(code);
            var tokenDomain = await _accessTokenServices.AddOrUpdate(accessToken);
            var jobParam = new JobParameter()
            {
                IsError = false,
                IsExecuted = false,
                JobName = nameof(JobSyncActivites),
                Parameter = JsonConvert.SerializeObject(new JobSyncActivityParameter()
                {
                    AccessToken = accessToken.Token,
                    FromDate = tokenDomain.IsExist ? null : _run4PrizeAppConfig.from_date
                })
            };
            await _dbContext.JobParameters.AddAsync(jobParam);
            await _dbContext.SaveChangesAsync();
            using (var scopeA = _serviceProvider.CreateScope())
            {
                var schedulerFactory = scopeA.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                var jobDataMap = new JobDataMap();
                jobDataMap.Put(JobSyncActivites.JobParamKey, true);
                await scheduler.TriggerJob(JobSyncActivites.jobKey, jobDataMap);
            }
            await CreateCookieAsync(tokenDomain.Athlete);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(string? error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewData["Error"] = error;
            } else
            {
                ViewData["Error"] = "";
            }
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [NonAction]
        private async Task CreateCookieAsync(AthleteDomain athleteDomain)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{athleteDomain.Id}"),
                new Claim("FullName", $"{athleteDomain.LastName} {athleteDomain.FirstName}"),
                new Claim("Avatar", $"{athleteDomain.ProfileMedium}"),
                new Claim(ClaimTypes.Role, "User"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}