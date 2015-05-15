using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.Models
{

    public class AddressBook
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("groups")]
        public List<int> GroupIds { get; set; }

        [JsonIgnore]
        public List<Group> Groups { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is AddressBook)
            {
                return ((AddressBook) obj).Id == this.Id;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
