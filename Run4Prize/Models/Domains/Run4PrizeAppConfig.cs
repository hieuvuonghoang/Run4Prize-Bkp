using System;

namespace Run4Prize.Models.Domains
{
    public class Run4PrizeAppConfig
    {
        public string? redirect_uri { get; set; }
        public string? client_id { get; set;}
        public string? client_secret { get; set;}
        public string? grant_type { get; set;}
        public string? response_type { get; set;}
        public DateTime? from_date { get; set; }
        public long user_admin { get; set; }
    }
}
