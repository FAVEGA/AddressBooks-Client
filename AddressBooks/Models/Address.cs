using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.Models
{
    public class Address
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("email")]
        private string Email;

        [JsonProperty("groups")]
        private List<int> GroupIds;
    }
}
