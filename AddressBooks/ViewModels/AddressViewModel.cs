using System.Collections.Generic;
using AddressBooks.Models;

namespace AddressBooks.ViewModels
{
    class AddressViewModel
    {
        public Address Address;

        public bool Changed { get; set; }

        public int Id
        {
            get { return Address.Id; }
            set {
                if (Address.Id != value)
                {
                    Changed = true;
                }
                Address.Id = value;
            }
        }

        public string Name
        {
            get { return Address.Name; }
            set
            {
                if (Address.Name != value)
                {
                    Changed = true;
                }
                Address.Name = value;
            }
        }

        public string Email
        {
            get { return Address.Email; }
            set
            {
                if (Address.Email != value)
                {
                    Changed = true;
                }
                Address.Email = value;
            }
        }

        public List<Group> Groups
        {
            get { return Address.Groups;  }
            set
            {
                if (Address.Groups != value)
                {
                    Changed = true;
                }
                Address.Groups = value;
            }
        }

        public AddressViewModel()
        {
            Address = new Address();
        }

        public AddressViewModel(Address address)
        {
            Address = address;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is AddressViewModel)
            {
                return ((AddressViewModel)obj).Id == Id;
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
