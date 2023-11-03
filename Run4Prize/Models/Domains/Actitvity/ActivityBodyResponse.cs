using Newtonsoft.Json;

namespace Run4Prize.Models.Domains.Actitvity
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Datum
    {
        public string? id { get; set; }
        public string? extId { get; set; }
        public string? site { get; set; }
        public string? name { get; set; }
        public double distance { get; set; }
        public double movingTime { get; set; }
        public double elapsedTime { get; set; }
        public string? type { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string? user { get; set; }
        public object? picture { get; set; }
        public object? pictureId { get; set; }
        public bool isManual { get; set; }
        public bool invalid { get; set; }
        [JsonProperty("checked ")]
        public bool isChecked { get; set; }
        public List<Split>? splits { get; set; }
        public string? map { get; set; }
        public double elevationGain { get; set; }
        public double avgHeartRate { get; set; }
        public double calories { get; set; }
        public string? deviceName { get; set; }
        public double steps { get; set; }
        public object? note { get; set; }
        public string? mapUrl { get; set; }
    }

    public class Split
    {
        public double distance { get; set; }
        public double movingTime { get; set; }
        public double elapsedTime { get; set; }
    }

    public class ActivityBodyResponse
    {
        public bool success { get; set; }
        public List<Datum>? data { get; set; }
    }

}
