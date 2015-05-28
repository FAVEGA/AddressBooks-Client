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
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Group> Groups { get; set; }
        public User Owner { get; set; }
        public List<User> SharedWith { get; set; } 

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
