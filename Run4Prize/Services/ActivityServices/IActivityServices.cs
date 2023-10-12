using com.strava.v3.api.Activities;
using Run4Prize.Models.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Run4Prize.Services.ActivityServices
{
    public interface IActivityServices
    {
        Task<List<ActivityDomain>> AddIfNoExists(List<ActivitySummary> activityDomains);
    }
}
