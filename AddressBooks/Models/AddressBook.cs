using System.Collections.Generic;

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
                return ((AddressBook) obj).Id == Id;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
