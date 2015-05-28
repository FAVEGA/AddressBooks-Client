using System.Collections.Generic;

namespace AddressBooks.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Group> Groups { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is AddressBook)
            {
                return ((AddressBook)obj).Id == Id;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
