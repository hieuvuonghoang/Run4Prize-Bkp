using System;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class WeekEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsActive { get; set; }
    }
}
