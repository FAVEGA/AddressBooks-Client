using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
