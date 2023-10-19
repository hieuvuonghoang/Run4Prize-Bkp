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
            if (!User.Identity!.IsAuthenticated)
            {
                ViewData[ConstantDomains.UrlCallBackStrava] = $"https://www.strava.com/oauth/authorize?client_id={_run4PrizeAppConfig.client_id}&response_type={_run4PrizeAppConfig.response_type}&redirect_uri={_run4PrizeAppConfig.redirect_uri}&approval_prompt=force&scope=read%2Cactivity%3Aread%2Cactivity%3Aread_all";
            }
            else
            {
                var claims = HttpContext.User.Claims;
                var claimAvatar = claims
                    .FirstOrDefault(x => x.Type == "Avatar")?.Value;
                var userId = claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                var accessToken = await _dbContext.Tokens
                    .Where(it => it.AthleteId == long.Parse(userId!))
                    .Select(it => new AccessTokenEntity()
                    {
                        token = it.token
                    })
                    .FirstOrDefaultAsync();
                if (accessToken != null)
                {
                    var jobParam = new JobParameter()
                    {
                        IsError = false,
                        IsExecuted = false,
                        JobName = nameof(JobSyncActivites),
                        Parameter = JsonConvert.SerializeObject(new JobSyncActivityParameter()
                        {
                            AccessToken = accessToken.token,
                            FromDate = null
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
                }
                ViewData[ConstantDomains.UrlUserAvatar] = claimAvatar;
            }
            var weekEntitys = await _dbContext
                .Weeks
                .Where(it => it.IsActive).AsNoTracking().ToListAsync();
            weekEntitys = weekEntitys.OrderBy(it => it.Id).ToList();
            if (weekEntitys == null || weekEntitys.Count == 0)
                return RedirectToAction(nameof(Error), new { error = "Tables Weeks empty!" });
            var sLWeek = new SelectList(weekEntitys, "Id", "Name");
            var utcNow = DateTime.UtcNow;
            var weekChoose = weekEntitys
                .Where(it => it.FromDate <= utcNow && it.ToDate >= utcNow && !it.Name!.ToUpper().Contains("TẤT CẢ"))
                .FirstOrDefault();
            var outWeek = 0;
            if (!string.IsNullOrEmpty(week) && int.TryParse(week, out outWeek))
            {
                weekChoose = weekEntitys.Where(it => it.Id == outWeek)
                    .FirstOrDefault();
            }
            if (weekChoose == null)
                weekChoose = weekEntitys.Where(it => it.Name!.ToUpper().Contains("TẤT CẢ")).FirstOrDefault();
            if (weekChoose == null)
                weekChoose = weekEntitys.FirstOrDefault();
            var utcTimeFromDate = new DateTimeWithZone(weekChoose!.FromDate, TimeZoneInfo.Utc).UniversalTime;
            var utcTimeToDate = new DateTimeWithZone(weekChoose.ToDate, TimeZoneInfo.Utc).UniversalTime;
            var localTimeFromDate = TimeZoneInfo.ConvertTime(utcTimeFromDate, TimeZoneInfo.Local);
            var localTimeToDate = TimeZoneInfo.ConvertTime(utcTimeToDate, TimeZoneInfo.Local);
            var query =
                from athlete in _dbContext.Athletes
                from activite in _dbContext.Activities
                where athlete.Id == activite.AthleteId && activite.StartDate >= localTimeFromDate && activite.StartDate <= localTimeToDate
                select new
                {
                    athlete = athlete,
                    activite = activite
                };
            var resultQuery = query.AsNoTracking().ToList();
            var groupByAthletes = resultQuery
                .GroupBy(it => it.athlete.Id)
                .ToList();
            var datas = new List<AthleteDomain>();
            var timeZone = TimeZoneInfo
                .GetSystemTimeZones().Where(it => it.BaseUtcOffset == TimeSpan.FromHours(7))
                .First();
            foreach (var groupByAthlete in groupByAthletes.ToList())
            {
                var athlete = _mapper.Map<AthleteEntity, AthleteDomain>(groupByAthlete.FirstOrDefault()!.athlete);
                athlete.Activities = new List<ActivityDomain>();
                foreach (var activite in groupByAthlete.ToList())
                {
                    activite.activite.StartDate = new DateTimeWithZone(activite.activite.StartDate, TimeZoneInfo.Local).UniversalTime;
                    activite.activite.StartDate = TimeZoneInfo.ConvertTime(activite.activite.StartDate, timeZone);
                    if (!activite.activite.Name!.ToUpper().Contains("WALK")
                        && !activite.activite.Name!.ToUpper().Contains("RUN"))
                    {
                        activite.activite.Distance = activite.activite.Distance / 3;
                    }
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
            var toDateTmp = TimeZoneInfo.ConvertTime(utcTimeToDate, timeZone);
            var fromDateTmp = TimeZoneInfo.ConvertTime(utcTimeFromDate, timeZone);
            var dates = new List<DateTime>();
            while (true)
            {
                var dateTmp = fromDateTmp.AddDays(i);
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
                chartDatasetDomain.Tension = 0f;
                chartDatasetDomain.BorderColor = athlete.ColorCode;
                chartDatasetDomain.Label = athlete.FirstName + " " + athlete.LastName;
                chartDatasetDomain.BorderWidth = 1;
                chartDatasetDomain.AthleteId = athlete.Id;
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
            dataOnes = dataOnes.OrderByDescending(it => it.TotalDistance)
                .ToList();
            var distanceTarget = 0F;
            if (weekChoose.DistanceTarget != null)
                distanceTarget = weekChoose.DistanceTarget.Value;
            else
                distanceTarget = dataOnes.Sum(it => it.DistanceOfWeek);
            ViewData["distanceTarget"] = distanceTarget.ToString("N2");
            ViewData["dayMonths"] = JsonConvert.SerializeObject(dayMonths);
            ViewData["DataOne"] = dataOnes;
            ViewData["chartDatasets"] = JsonConvert.SerializeObject(chartDatasetDomains);
            ViewData["WeekId"] = weekChoose.Id.ToString();
            ViewData["WeekName"] = weekChoose.Name;
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
        public async Task<IActionResult> SyncActivity()
        {
            var claims = HttpContext.User.Claims;
            var userId = claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var activities = await _dbContext.Activities.Where(it => it.AthleteId == long.Parse(userId!)).ToListAsync();
            var tokens = await _dbContext.Tokens.Where(it => it.AthleteId == long.Parse(userId!)).ToListAsync();
            var weekUserDistances = await _dbContext.WeekUserDistances.Where(it => it.AthleteId == long.Parse(userId!)).ToListAsync();
            var athletes = await _dbContext.Athletes.Where(it => it.Id == long.Parse(userId!)).ToListAsync();
            foreach (var item in activities)
                _dbContext.Entry(item).State = EntityState.Deleted;
            foreach (var item in tokens)
                _dbContext.Entry(item).State = EntityState.Deleted;
            foreach (var item in weekUserDistances)
                _dbContext.Entry(item).State = EntityState.Deleted;
            foreach (var item in athletes)
                _dbContext.Entry(item).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
            await HttpContext.SignOutAsync();
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
            var isAdmin = claims
                .Where(x => x.Type == ClaimTypes.Role && x.Value == "Administrator")
                .FirstOrDefault() != null ? true : false;
            if(!isAdmin)
            {
                if (model.AthleteId == 0 || model.WeekId == 0 || userId != model.AthleteId.ToString())
                {
                    return RedirectToAction(nameof(Error), new { error = "Forbidden!" });
                }
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
                await _dbContext.WeekUserDistances.AddAsync(_mapper.Map<WeekUserDistanceDomain, WeekUserDistanceEntity>(model));
            else
                weekUserDistances.Distance = model.Distance;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { week = model.WeekId });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [TypeFilter(typeof(CustomAuthorizationFilterAttribute))]
        public async Task<IActionResult> Color(AthleteDomain model)
        {
            var claims = HttpContext.User.Claims;
            var userId = claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var isAdmin = claims
                .Where(x => x.Type == ClaimTypes.Role && x.Value == "Administrator")
                .FirstOrDefault() != null ? true : false;
            if (!isAdmin)
            {
                if (model.Id == 0 || userId != model.Id.ToString())
                {
                    return RedirectToAction(nameof(Error), new { error = "Forbibden!" });
                }
            }
            var weekUserDistances = _dbContext
                .Athletes
                .Where(it => it.Id == model.Id)
                .FirstOrDefault();
            if (weekUserDistances == null)
            {
                return RedirectToAction(nameof(Error), new { error = "WeekUserDistances is null!" });
            }
            else
            {
                weekUserDistances.ColorCode = model.ColorCode;
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
            await CreateCookieAsync(tokenDomain.Athlete);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [TypeFilter(typeof(CustomAuthorizationFilterAttribute))]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Logs(int skip, int take)
        {
            skip = skip == 0 ? 0 : skip;
            take = take == 0 ? 20 : take;
            var logs = await _dbContext.Logs
                .OrderByDescending(it => it.Id)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
            return Ok(JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> JobParams(int skip, int take)
        {
            skip = skip == 0 ? 0 : skip;
            take = take == 0 ? 20 : take;
            var jobParameters = await _dbContext.JobParameters
                .OrderByDescending(it => it.Id)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
            return Ok(JsonConvert.SerializeObject(jobParameters, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> Tokens(int skip, int take)
        {
            skip = skip == 0 ? 0 : skip;
            take = take == 0 ? 20 : take;
            var tokens = await _dbContext.Tokens
                .OrderByDescending(it => it.Id)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
            return Ok(JsonConvert.SerializeObject(tokens, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> Activities(int skip, int take)
        {
            skip = skip == 0 ? 0 : skip;
            take = take == 0 ? 20 : take;
            var activities = await _dbContext.Activities
                .OrderByDescending(it => it.Id)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
            return Ok(JsonConvert.SerializeObject(activities, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> Athletes(int skip, int take)
        {
            skip = skip == 0 ? 0 : skip;
            take = take == 0 ? 20 : take;
            var athletes = await _dbContext.Athletes
                .OrderByDescending(it => it.Id)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
            return Ok(JsonConvert.SerializeObject(athletes, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> ClearLogs()
        {
            var logs = await _dbContext.Logs
                .ToListAsync();
            foreach (var log in logs)
            {
                _dbContext.Entry(log).State = EntityState.Deleted;
            }
            var count = await _dbContext.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(new { RowCount = logs.Count, Deleted = count }, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> ClearJobParams()
        {
            var entities = await _dbContext.JobParameters
                .ToListAsync();
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            var count = await _dbContext.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(new { RowCount = entities.Count, Deleted = count }, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> ClearTokens()
        {
            var entities = await _dbContext.Tokens
                .ToListAsync();
            foreach (var entity in entities)
                _dbContext.Entry(entity).State = EntityState.Deleted;
            var count = await _dbContext.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(new { RowCount = entities.Count, Deleted = count }, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> ClearActivities()
        {
            var entities = await _dbContext.Activities
                .ToListAsync();
            foreach (var entity in entities)
                _dbContext.Entry(entity).State = EntityState.Deleted;
            var count = await _dbContext.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(new { RowCount = entities.Count, Deleted = count }, Formatting.Indented));
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> TriggerJobRefreshToken()
        {
            using (var scopeA = _serviceProvider.CreateScope())
            {
                var schedulerFactory = scopeA.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await scheduler.TriggerJob(JobRefreshToken.jobKey);
            }
            return Ok("TriggerRefreshToken Done!");
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> TriggerJobSyncActivities(string? fromDateStr)
        {
            DateTime? fromDate = null;
            if (!string.IsNullOrEmpty(fromDateStr))
            {
                try
                {
                    fromDate = DateTime.ParseExact(fromDateStr, "yyyyMMdd", null);
                }
                catch (Exception)
                {
                }
            }
            var tokens = await _dbContext.Tokens.Select(it => it.token)
                .ToListAsync();
            foreach (var token in tokens)
            {
                var jobParam = new JobParameter()
                {
                    IsError = false,
                    IsExecuted = false,
                    JobName = nameof(JobSyncActivites),
                    Parameter = JsonConvert.SerializeObject(new JobSyncActivityParameter()
                    {
                        AccessToken = token!,
                        FromDate = fromDate
                    })
                };
                await _dbContext.JobParameters.AddAsync(jobParam);
            }
            await _dbContext.SaveChangesAsync();
            using (var scopeA = _serviceProvider.CreateScope())
            {
                var schedulerFactory = scopeA.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                var jobDataMap = new JobDataMap();
                jobDataMap.Put(JobSyncActivites.JobParamKey, true);
                await scheduler.TriggerJob(JobSyncActivites.jobKey, jobDataMap);
            }
            return Ok("TriggerJobSyncActivities Done!");
        }

        [HttpGet]
        [CustomAuthorizationFilterAttribute(Roles = "Administrator")]
        public async Task<IActionResult> DisableWeekTrainings()
        {
            var weeks = await _dbContext.Weeks
                .Where(it => it.Name!.ToUpper().Contains("TẬP LUYỆN"))
                .ToListAsync();
            foreach (var week in weeks)
            {
                week.IsActive = false;
            }
            await _dbContext.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(weeks, Formatting.Indented));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(string? error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewData["Error"] = error;
            }
            else
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
                new Claim("FullName", $"{athleteDomain.FirstName} {athleteDomain.LastName}"),
                new Claim("Avatar", $"{athleteDomain.ProfileMedium}"),
                new Claim(ClaimTypes.Role, "User"),
            };
            if (athleteDomain.Id == _run4PrizeAppConfig.user_admin)
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(20),
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