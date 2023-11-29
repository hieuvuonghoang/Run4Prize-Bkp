namespace Run4Prize.Models.DBContexts.AppContext
{
    public class Activity
    {
        public long Id { get; set; }
        public long MemberId { get; set; }
        public string? Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public double Distance { get; set; }
    }
}
