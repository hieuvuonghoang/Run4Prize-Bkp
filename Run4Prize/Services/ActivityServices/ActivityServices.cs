using AutoMapper;
using com.strava.v3.api.Activities;
using Microsoft.EntityFrameworkCore;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Run4Prize.Services.ActivityServices
{
    public class ActivityServices : IActivityServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        public ActivityServices(AppDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<List<ActivityDomain>> AddIfNoExists(List<ActivitySummary> activityDomains)
        {
            var activityEntites = _mapper.Map<List<ActivitySummary>, List<ActivityEntity>>(activityDomains);
            var isSave = false;
            foreach (var activityEntity in activityEntites)
            {
                var findEntity = await _dbContext.Activities
                    .Where(it => it.Id == activityEntity.Id)
                    .Select(it => new ActivityEntity()
                    {
                        Id = it.Id
                    })
                    .FirstOrDefaultAsync();
                if (findEntity == null)
                {
                    activityEntity.EntityCreateDate = activityEntity.EntityUpdateDate = DateTime.UtcNow;
                    await _dbContext.Activities.AddAsync(activityEntity);
                    isSave = true;
                }
            }
            if(isSave)
                await _dbContext.SaveChangesAsync();
            return _mapper.Map<List<ActivityEntity>, List<ActivityDomain>>(activityEntites);
        }
    }
}
