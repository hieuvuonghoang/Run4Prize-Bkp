namespace Run4Prize.Models.DBContexts.AppContext
{
    public class WeekUserDistanceEntity
    {
        public long Id { get; set; }
        public long WeekId { get; set; }
        public long AthleteId { get; set; }
        public float Distance { get; set; }

        public AthleteEntity? Athlete { get; set; }
        public WeekEntity? Week { get; set; }
    }
}
