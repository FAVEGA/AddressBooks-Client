using System.Collections.Generic;
using Newtonsoft.Json;

namespace AddressBooks.Rest
{
    public class RestAddress
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("groups")]
        public List<int> GroupIds;
    }
}
