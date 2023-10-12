
using Newtonsoft.Json;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class AccessTokenEntity : BaseEntity
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("access_token")]
        public string? token { get; set; }
        [JsonProperty("expires_at")]
        public string? expiresat { get; set; }
        [JsonProperty("expires_in")]
        public string? expiresin { get; set; }
        [JsonProperty("refresh_token")]
        public string? refreshtoken { get; set; }
        [JsonProperty("athlete_id")]
        public long AthleteId { get; set; }
        [JsonProperty("utc_expire_at")]
        public DateTime utcexpireat { get; set; }

        [JsonProperty("athlete")]
        public AthleteEntity? Athlete {  get; set; }
    }
}
