namespace Run4Prize.Models.DBContexts.AppContext
{
    public class Distance
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public double TotalDistance { get; set; }
        public long MemberId { get; set; }
    }
}
