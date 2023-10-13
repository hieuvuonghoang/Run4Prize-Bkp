using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Run4Prize.Models.Domains
{
    public class AthleteDomain : BaseDomain
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
        public AccessTokenDomain? AccessToken { get; set; }

        [JsonProperty("activities")]
        public List<ActivityDomain>? Activities { get; set; }

        [JsonProperty("distance-of-week")]
        public float DistanceOfWeek { get; set; }

        [JsonProperty("total-distance")]
        public float TotalDistance
        {
            get
            {
                float result = 0;
                if (Activities != null)
                {
                    result = Activities.Sum(it => it.Distance) / 1000;
                }
                return result;
            }
        }

        [JsonProperty("total-time")]
        public float TotalTime
        {
            get
            {
                float result = 0;
                if (Activities != null)
                {
                    result = Activities.Sum(it => it.MovingTime) / 60;
                }
                return result;
            }
        }

        public float Percent
        {
            get
            {
                if (DistanceOfWeek == 0)
                    return 0;
                return (TotalDistance / DistanceOfWeek) * 100;
            }
        }

        public float Pace
        {
            get
            {
                if (TotalDistance == 0)
                    return 0;
                else
                    return TotalTime / TotalDistance;
            }
        }

        public long WeekId { get; set; }
    }
}
