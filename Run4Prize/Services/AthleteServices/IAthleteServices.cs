using com.strava.v3.api.Athletes;
using Run4Prize.Models.Domains;
using System.Threading.Tasks;

namespace Run4Prize.Services.AthleteServices
{
    public interface IAthleteServices
    {
        Task<AthleteDomain> AddIfNoExists(Athlete athlete);
    }
}
