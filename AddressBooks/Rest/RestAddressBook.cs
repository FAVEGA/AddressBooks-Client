using System.Collections.Generic;
using Newtonsoft.Json;

namespace AddressBooks.Rest
{
    public class RestAddressBook
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("groups")]
        public List<int> GroupIds { get; set; }

        [JsonProperty("owner")]
        public int OwnerId { get; set; }

        [JsonProperty("shared_with")]
        public List<int> SharedWithIds { get; set; }
    }
}
