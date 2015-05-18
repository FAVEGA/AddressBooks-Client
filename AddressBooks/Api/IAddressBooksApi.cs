using AddressBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.Api
{
    interface IAddressBooksApi
    {
        List<AddressBook> GetAddressBooks();
        AddressBook GetAddressBook(int id);
        void CreateAddressBook(AddressBook addressBook);
        void DeleteAddressBook(AddressBook addressBook);
        void UpdateAddressBook(AddressBook addressBook);

        List<Group> GetGroups();
        Group GetGroup(int id);
        void CreateGroup(Group group);
        void DeleteGroup(Group group);
        void UpdateGroup(Group group);

        List<Address> GetAddresses();
        Address GetAddress(int id);
        void CreateAddress(Address address);
        void DeleteAddress(Address address);
        void UpdateAddress(Address address);
    }
}
