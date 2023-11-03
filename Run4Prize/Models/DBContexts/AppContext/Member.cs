using System.ComponentModel.DataAnnotations.Schema;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class Member
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long TeamId { get; set; }
        [NotMapped]
        public List<Distance>? Distances { get; set; }
    }
}
