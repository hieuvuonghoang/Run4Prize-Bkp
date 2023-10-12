using com.strava.v3.api.Activities;
using com.strava.v3.api.Athletes;
using com.strava.v3.api.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Run4Prize.Services
{
    public interface IStravaServices
    {
        Task<AccessToken> Authentication(string code);
        Task<List<ActivitySummary>> GetActivities(string code, DateTime fromDateTime);
        Task<AccessToken> RefreshToken(AccessToken accessToken);
    }
}
