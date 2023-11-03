using Microsoft.AspNetCore.Mvc;
using Run4Prize.Models.DBContexts.AppContext;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Run4Prize.Enum;
using Quartz;
using Run4Prize.Jobs;
using Newtonsoft.Json;

namespace Run4Prize.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public HomeController(ILogger<HomeController> logger,
            AppDBContext dbContext,
            IServiceProvider serviceProvider,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> Trigger()
        {
            using (var scopeA = _serviceProvider.CreateScope())
            {
                var schedulerFactory = scopeA.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await scheduler.TriggerJob(JobSyncData.jobKey);
            }
            return Ok("Trigger done!");
        }

        public IActionResult UpdateSettings(int type, string value)
        {
            var find = _dbContext.Settings.Where(it => it.Type == type).FirstOrDefault();
            if (find != null)
            {
                find.Value = value;
                _dbContext.SaveChanges();
                return Ok("UpdateSettings done!");
            } else
            {
                return Ok("Not found setting!");
            }
        }

        public async Task<IActionResult> Logs()
        {
            var logs = await _dbContext.Logs.OrderByDescending(it => it.Id).Skip(0).Take(10).ToListAsync();
            return Ok(JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        public async Task<IActionResult> Index(string dats)
        {
            var timeZoneVN = TimeZoneInfo
               .GetSystemTimeZones()
               .Where(it => it.BaseUtcOffset == TimeSpan.FromHours(7))
               .First();
            var nowVN = TimeZoneInfo.ConvertTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), timeZoneVN);
            DateTime toDate = TimeZoneInfo.ConvertTime(new DateTime(nowVN.Year, nowVN.Month, nowVN.Day), timeZoneVN);
            if(nowVN.DayOfWeek != DayOfWeek.Monday)
            {
                while (true)
                {
                    toDate = toDate.AddDays(-1);
                    if (toDate.DayOfWeek == DayOfWeek.Monday)
                    {
                        break;
                    }
                }
            }
            if(!string.IsNullOrEmpty(dats))
            {
                DateTime.TryParse(dats, out toDate);
                toDate = TimeZoneInfo.ConvertTime(toDate, timeZoneVN);
            }
            var teams = await _dbContext.Teams.AsNoTracking().ToListAsync();
            var members = await _dbContext.Members.AsNoTracking().ToListAsync();
            var distances = await _dbContext.Distances.Where(it => it.CreateDate >= toDate).AsNoTracking().ToListAsync();
            teams = teams.OrderBy(it => it.Rank).ToList();
            foreach(var team in teams)
            {
                team.Members = members
                    .Where(it => it.TeamId ==  team.Id)
                    .ToList();
                foreach(var member in members)
                {
                    member.Distances = distances
                        .Where(it => it.MemberId == member.Id)
                        .ToList();
                    if(member.Distances == null)
                    {
                        member.Distances = new List<Distance>();
                    }
                }
                team.Members = team.Members.OrderByDescending(it => it.Distances!.Sum(x => x.TotalDistance)).ToList();
            }
            var teamSetting = _dbContext
                .Settings
                .Where(it => it.Type == (int)EnumSetting.TeamId)
                .AsNoTracking()
                .FirstOrDefault();
            ViewData["teamId"] = teamSetting!.Value;
            ViewData["data"] = teams;
            ViewData["toDate"] = toDate;
            ViewData["fromDate"] = nowVN;
            return View();
        }
    }
}