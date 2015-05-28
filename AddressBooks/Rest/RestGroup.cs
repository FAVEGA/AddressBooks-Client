using System.Collections.Generic;
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
