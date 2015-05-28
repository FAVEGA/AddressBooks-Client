using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressBooks.Models;

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
