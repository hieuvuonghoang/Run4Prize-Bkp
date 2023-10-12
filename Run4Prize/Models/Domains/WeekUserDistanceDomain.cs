using Run4Prize.Models.DBContexts.AppContext;

namespace Run4Prize.Models.Domains
{
    public class WeekUserDistanceDomain
    {
        public long Id { get; set; }
        public long WeekId { get; set; }
        public long AthleteId { get; set; }
        public float Distance { get; set; }

        public AthleteDomain? Athlete { get; set; }
        public WeekDomain? Week { get; set; }
    }
}
