using Newtonsoft.Json;

namespace Run4Prize.Models.Domains
{
    public class ChartDatasetDomain
    {
        [JsonProperty("data")]
        public List<float>? Data { get; set; }
        [JsonProperty("borderColor")]
        public string? BorderColor { get; set; }
        [JsonProperty("fill")]
        public bool Fill { get; set; }
        [JsonProperty("label")]
        public string? Label { get; set; }
        [JsonProperty("tension")]
        public float Tension { get; set; }
    }
}
