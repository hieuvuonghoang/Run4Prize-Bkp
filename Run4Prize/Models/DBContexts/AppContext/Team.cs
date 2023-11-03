using System.ComponentModel.DataAnnotations.Schema;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class Team
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public double Rank { get; set; }
        public string? Uid { get; set; }
        [NotMapped]
        public List<Member>? Members { get; set; }
    }
}
