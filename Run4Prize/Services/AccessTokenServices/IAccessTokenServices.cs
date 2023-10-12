using com.strava.v3.api.Activities;
using com.strava.v3.api.Athletes;
using com.strava.v3.api.Authentication;
using Run4Prize.Models.Domains;
using System.Threading.Tasks;

namespace Run4Prize.Services.AccessTokenServices
{
    public interface IAccessTokenServices
    {
        Task<AccessTokenDomain> AddOrUpdate(AccessToken accessToken);
    }
}
