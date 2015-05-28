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
        Task<AddressBook> CreateAddressBook(AddressBook addressBook);
        Task DeleteAddressBook(AddressBook addressBook);
        Task ChangeAddressBook(AddressBook addressBook);

        List<Group> GetGroups();
        Group GetGroup(int id);
        Task<Group> CreateGroup(Group group);
        Task DeleteGroup(Group group);
        Task ChangeGroup(Group group);

        List<Address> GetAddresses();
        Address GetAddress(int id);
        Address GetAddress(string email, int addressBookId);
        Task<Address> CreateAddress(Address address);
        Task DeleteAddress(Address address);
        Task ChangeAddress(Address address);

        List<User> GetUsers();
        User GetUser(int id);
    }
}
