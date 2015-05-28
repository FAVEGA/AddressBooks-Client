using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AddressBooks.Rest
{
    public class RestGroup
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("name")] public string Name;
        [JsonProperty("addressbook")] public int AddressBook;
        [JsonProperty("addresses")] public List<int> Addresses;
    }
}
