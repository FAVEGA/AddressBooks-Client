using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.Models
{
    public class Group
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("address_book")]
        private int AddressBookId;

        [JsonProperty("ignore")]
        public List<Address> Addresses;
    }
}
