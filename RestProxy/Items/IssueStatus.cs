using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCDefectIntegration.RestProxy.Items
{
    /// <summary>
    /// Class representing the status of an issue
    /// </summary>
    public class IssueStatus
    {

        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("statusCategory")]
        public Dictionary<string,string> statusCategoryValues { get; set; }


    }
}
