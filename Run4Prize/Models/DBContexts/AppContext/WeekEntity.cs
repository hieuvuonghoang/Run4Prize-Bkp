using Newtonsoft.Json;
using System;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class WeekEntity
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
        [JsonProperty("distance-target")]
        public float? DistanceTarget { get; set; }

        [JsonProperty("week-user-distances")]
        public ICollection<WeekUserDistanceEntity>? WeekUserDistances { get; set; }
    }
}
