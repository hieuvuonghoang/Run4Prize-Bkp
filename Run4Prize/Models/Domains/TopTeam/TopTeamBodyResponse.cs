namespace Run4Prize.Models.Domains.TopTeam
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public List<Result>? result { get; set; }
        public double total { get; set; }
        public double skip { get; set; }
        public double limit { get; set; }
    }

    public class DistanceByTypes
    {
        public double Run { get; set; }
        public double Walk { get; set; }
        public double Bike { get; set; }
    }

    public class Result
    {
        public string? teamId { get; set; }
        public string? name { get; set; }
        public double rank { get; set; }
        public double distance { get; set; }
        public double totalTime { get; set; }
        public double members { get; set; }
        public double totalMembers { get; set; }
        public string? avatar { get; set; }
        public DistanceByTypes? distanceByTypes { get; set; }
        public double avgMemDist { get; set; }
        public double donated { get; set; }
        public string? subLevel { get; set; }
    }

    public class TopTeamBodyResponse
    {
        public bool success { get; set; }
        public Data? data { get; set; }
    }

}
