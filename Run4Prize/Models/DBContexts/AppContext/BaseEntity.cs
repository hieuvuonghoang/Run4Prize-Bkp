using Newtonsoft.Json;
using System;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class BaseEntity
    {
        [JsonProperty("create_date")]
        public DateTime EntityCreateDate { get; set; }
        [JsonProperty("update_date")]
        public DateTime EntityUpdateDate { get; set; }
    }
}
