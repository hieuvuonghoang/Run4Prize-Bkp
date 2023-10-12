namespace Run4Prize.Models.DBContexts.AppContext
{
    public class LogEntity : BaseEntity
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
    }
}
