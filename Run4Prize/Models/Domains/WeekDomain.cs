using Newtonsoft.Json;

namespace Run4Prize.Models.Domains
{
    public class WeekDomain
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("from-date")]
        public DateTime FromDate { get; set; }
        [JsonProperty("to-date")]
        public DateTime ToDate { get; set; }
        [JsonProperty("is-active")]
        public bool IsActive { get; set; }

        [JsonProperty("week-user-distances")]
        public ICollection<WeekUserDistanceDomain>? WeekUserDistances { get; set; }
    }
}
