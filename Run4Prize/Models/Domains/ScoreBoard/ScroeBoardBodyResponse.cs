using Newtonsoft.Json;

namespace Run4Prize.Models.Domains.ScoreBoard
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public double total { get; set; }
        public List<Result>? result { get; set; }
    }

    public class DistanceByTypes
    {
        public double Run { get; set; }
        public double Bike { get; set; }
        public double Walk { get; set; }
    }

    public class MoreInfos
    {
        [JsonProperty("Mã nhân viên")]
        public string? maNhanVien { get; set; }
        public string? teamBoard { get; set; }
    }

    public class Option
    {
        public string? id { get; set; }
        public string? type { get; set; }
        public string? typeName { get; set; }
        public string? name { get; set; }
        public bool showInBoard { get; set; }
    }

    public class OptionsMap
    {
        [JsonProperty("công ty")]
        public string? congTy { get; set; }
    }

    public class PersonalInfo
    {
        public string? gender { get; set; }
        public DateTime? dob { get; set; }
        public MoreInfos? moreInfos { get; set; }
    }

    public class Result
    {
        public string? uId { get; set; }
        public double userId { get; set; }
        public string? racerId { get; set; }
        public string? avatar { get; set; }
        public string? name { get; set; }
        public string? shortName { get; set; }
        public string? bib { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public double totalDistance { get; set; }
        public double complete { get; set; }
        public double distance { get; set; }
        public double time { get; set; }
        public double avgPace { get; set; }
        public double avgSpeed { get; set; }
        public double rank { get; set; }
        public double charity { get; set; }
        public string? club { get; set; }
        public string? teamId { get; set; }
        public string? subLevel { get; set; }
        public bool completed { get; set; }
        public PersonalInfo? personalInfo { get; set; }
        public string? country { get; set; }
        public DistanceByTypes? distanceByTypes { get; set; }
        public List<Option>? options { get; set; }
        public OptionsMap? optionsMap { get; set; }
        public double totalPodoubles { get; set; }
    }

    public class ScroeBoardBodyResponse
    {
        public bool success { get; set; }
        public Data? data { get; set; }
    }


}
