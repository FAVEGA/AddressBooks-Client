using System.Collections.Generic;
using AddressBooks.Models;
using Newtonsoft.Json;

namespace AddressBooks.Rest
{
    public class RestUsersPage
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<User> Results;
    }
}
