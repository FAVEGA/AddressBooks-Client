using System.Collections.Generic;
using Newtonsoft.Json;

namespace AddressBooks.Rest
{
    public class RestGroupsPage
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<RestGroup> Results;
    }
}
