using AutoMapper;
using com.strava.v3.api.Authentication;
using com.strava.v3.api.Clients;
using Microsoft.EntityFrameworkCore;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains;
using Run4Prize.Services.AthleteServices;
using System.Linq;
using System.Threading.Tasks;

namespace Run4Prize.Services.AccessTokenServices
{
    public class AccessTokenServices : IAccessTokenServices
    {
        private readonly IAthleteServices _athleteServices;
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;

        public AccessTokenServices(IAthleteServices athleteServices, AppDBContext dbContext, IMapper mapper)
        {
            _athleteServices = athleteServices;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<AccessTokenDomain> AddOrUpdate(AccessToken accessToken)
        {
            var auth = new StaticAuthentication(accessToken.Token);
            var client = new StravaClient(auth);
            var athlete = await client.Athletes.GetAthleteAsync();
            var athleteDomain = await _athleteServices.AddIfNoExists(athlete);
            var accessTokenEntity = await _dbContext.Tokens
                .Where(it => it.AthleteId == athlete.Id)
                .FirstOrDefaultAsync();
            var isExist = false;
            if (accessTokenEntity != null)
            {
                accessTokenEntity.token = accessToken.Token;
                accessTokenEntity.expiresat = accessToken.ExpiresAt;
                accessTokenEntity.expiresin = accessToken.ExpiresIn;
                accessTokenEntity.refreshtoken = accessToken.RefreshToken;
                isExist = true;
                await _dbContext.SaveChangesAsync();
            } else
            {
                accessTokenEntity = _mapper.Map<AccessToken, AccessTokenEntity>(accessToken);
                accessTokenEntity.AthleteId = athlete.Id;
                await _dbContext.Tokens.AddAsync(accessTokenEntity);
                await _dbContext.SaveChangesAsync();
            }
            var result = _mapper.Map<AccessTokenEntity, AccessTokenDomain>(accessTokenEntity);
            result.Athlete = athleteDomain;
            result.IsExist = isExist;
            return result;
        }
    }
}
