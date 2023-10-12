using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class AthleteEntity : BaseEntity
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }
        [JsonProperty("firstname")]
        public string? FirstName { get; set; }
        [JsonProperty("lastname")]
        public string? LastName { get; set; }
        [JsonProperty("profile_medium")]
        public string? ProfileMedium { get; set; }
        [JsonProperty("profile")]
        public string? Profile { get; set; }
        [JsonProperty("city")]
        public string? City { get; set; }
        [JsonProperty("state")]
        public string? State { get; set; }
        [JsonProperty("country")]
        public string? Country { get; set; }
        [JsonProperty("sex")]
        public string? Sex { get; set; }
        [JsonProperty("friend")]
        public string? Friend { get; set; }
        [JsonProperty("follower")]
        public string? Follower { get; set; }
        [JsonProperty("premium")]
        public bool IsPremium { get; set; }
        [JsonProperty("created_at")]
        public string? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string? UpdatedAt { get; set; }
        [JsonProperty("approve_followers")]
        public bool ApproveFollowers { get; set; }
        [JsonProperty("color_code")]
        public string? ColorCode { get; set; }
        [JsonProperty("access-token")]
        public AccessTokenEntity? AccessToken { get; set; }

        [JsonProperty("activities")]
        public ICollection<ActivityEntity>? Activities { get; set; }
    }
}
