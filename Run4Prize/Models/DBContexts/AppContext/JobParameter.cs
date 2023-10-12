using System;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class JobParameter
    {
        public int Id { get; set; }
        public string? JobName { get; set; }
        public string? Parameter { get; set; }
        public DateTime? ExecuteStartDate { get; set; }
        public DateTime? ExecuteEndDate { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsError { get; set; }
    }
}
