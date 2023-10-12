using Newtonsoft.Json;

namespace Run4Prize.Models.Domains
{
    public class ActivityDomain : BaseDomain
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("external_id")]
        public string? ExternalId { get; set; }
        [JsonProperty("type")]
        private string? _type { get; set; }
        [JsonProperty("sport_type")]
        private string? _sport_type { get; set; }
        [JsonProperty("distance")]
        public float Distance { get; set; }
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }
        [JsonProperty("total_elevation_gain")]
        public float ElevationGain { get; set; }
        [JsonProperty("has_kudoed")]
        public bool HasKudoed { get; set; }
        [JsonProperty("average_heartrate")]
        public float AverageHeartrate { get; set; }
        [JsonProperty("max_heartrate")]
        public float MaxHeartrate { get; set; }
        [JsonProperty("truncated")]
        public int? Truncated { get; set; }
        [JsonProperty("city")]
        public string? City { get; set; }
        [JsonProperty("state")]
        public string? State { get; set; }
        [JsonProperty("country")]
        public string? Country { get; set; }
        [JsonProperty("gear_id")]
        public string? GearId { get; set; }
        [JsonProperty("average_speed")]
        public float AverageSpeed { get; set; }
        [JsonProperty("max_speed")]
        public float MaxSpeed { get; set; }
        [JsonProperty("average_cadence")]
        public float AverageCadence { get; set; }
        [JsonProperty("average_temp")]
        public float AverageTemperature { get; set; }
        [JsonProperty("average_watts")]
        public float AveragePower { get; set; }
        [JsonProperty("kilojoules")]
        public float Kilojoules { get; set; }
        [JsonProperty("trainer")]
        public bool IsTrainer { get; set; }
        [JsonProperty("commute")]
        public bool IsCommute { get; set; }
        [JsonProperty("manual")]
        public bool IsManual { get; set; }
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("flagged")]
        public bool IsFlagged { get; set; }
        [JsonProperty("achievement_count")]
        public int AchievementCount { get; set; }
        [JsonProperty("kudos_count")]
        public int KudosCount { get; set; }
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty("athlete_count")]
        public int AthleteCount { get; set; }
        [JsonProperty("photo_count")]
        public int PhotoCount { get; set; }
        [JsonProperty("start_date")]
        public string? StartDate { get; set; }
        [JsonProperty("start_date_local")]
        public string? StartDateLocal { get; set; }
        [JsonProperty("timezone")]
        public string? TimeZone { get; set; }
        [JsonProperty("weighted_average_watts")]
        public int WeightedAverageWatts { get; set; }
        [JsonProperty("device_watts")]
        public bool HasPowerMeter { get; set; }
        [JsonProperty("suffer_score")]
        public double SufferScore { get; set; }
        [JsonProperty("calories")]
        public float Calories { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("athlete-id")]
        public long AthleteId { get; set; }
        [JsonProperty("year")]
        public int Year { get; set; }
        [JsonProperty("month")]
        public int Month { get; set; }
        [JsonProperty("day")]
        public int Day { get; set; }

        [JsonProperty("athlete")]
        public AthleteDomain? Athlete { get; set; }
    }
}
