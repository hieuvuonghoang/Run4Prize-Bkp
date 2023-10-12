using AutoMapper;
using com.strava.v3.api.Athletes;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains;
using System.Threading.Tasks;

namespace Run4Prize.Services.AthleteServices
{
    public class AthleteServices : IAthleteServices
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;

        public AthleteServices(AppDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<AthleteDomain> AddIfNoExists(Athlete athlete)
        {
            var athleteEntiy = _mapper.Map<Athlete, AthleteEntity>(athlete);
            var athleteEntiyExist = await _dbContext.Athletes.FindAsync(athleteEntiy.Id);
            if (athleteEntiyExist == null)
            {
                var random = new Random();
                var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
                athleteEntiy.ColorCode = color;
                var resultAdd = await _dbContext.Athletes.AddAsync(athleteEntiy);
                await _dbContext.SaveChangesAsync();
                athleteEntiyExist = resultAdd.Entity;
            }
            var result = _mapper.Map<AthleteEntity, AthleteDomain>(athleteEntiyExist);
            return result;
        }
    }
}

