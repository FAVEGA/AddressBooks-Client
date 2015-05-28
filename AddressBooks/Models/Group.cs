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
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Address> Addresses { get; set; }
        public AddressBook AddressBook { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Group)
            {
                return ((Group)obj).Id == this.Id;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
