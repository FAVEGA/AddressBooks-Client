using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AddressBooks.Models;
using AddressBooks.Rest;
using Refit;
using Group = AddressBooks.Models.Group;

namespace AddressBooks.Api
{
    class CachedAddressBooksApi : IAuthenticatedAddressBooksApi, IRegistrar, IUpdatable
    {

        private string _token;
        private IRestAddressBookApi _api;
        private List<AddressBook> _addressBooksCache = new List<AddressBook>();
        private List<User> _usersCache = new List<User>(); 
        private DateTime _lastChange;
        private readonly List<IRegisterable> _registerables = new List<IRegisterable>();

        public CachedAddressBooksApi()
        {
            _api = RestService.For<IRestAddressBookApi>("http://addressbooks.favega.com/");
        }

        public List<AddressBook> GetAddressBooks()
        {
            return _addressBooksCache;
        }

        public AddressBook GetAddressBook(int id)
        {
            if (_addressBooksCache.Find(o => o.Id == id) == null)
            {
                throw new Exception("Address book not found");
            }
            return _addressBooksCache.Find(o => o.Id == id);
        }

        public async Task<AddressBook> CreateAddressBook(AddressBook addressBook)
        {
            var restAddressBook = new RestAddressBook
            {
                Name = addressBook.Name,
                OwnerId = addressBook.Owner.Id,
                SharedWithIds = addressBook.SharedWith.Select(user => user.Id).ToList(),
                GroupIds = addressBook.Groups.Select(group => group.Id).ToList()
            };
            addressBook.Id = (await _api.CreateAddressBook(restAddressBook, "token " + _token)).Id;
            _addressBooksCache.Add(addressBook);
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Created address book " + addressBook.Id);
            return addressBook;
        }

        public async Task DeleteAddressBook(AddressBook addressBook)
        {
            await _api.DeleteAddressBook(addressBook.Id, "token " + _token);
            _addressBooksCache.Remove(addressBook);
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Deleted address book " + addressBook.Id);
        }

        public async Task ChangeAddressBook(AddressBook addressBook)
        {
            _addressBooksCache.Remove(addressBook);
            var restAddressBook = new RestAddressBook
            {
                Id = addressBook.Id,
                Name = addressBook.Name,
                OwnerId = addressBook.Owner.Id,
                SharedWithIds = addressBook.SharedWith.Select(user => user.Id).ToList(),
                GroupIds = addressBook.Groups.Select(group => group.Id).ToList()
            };
            await _api.ChangeAddressBook(addressBook.Id, restAddressBook, "token " + _token);
            _addressBooksCache.Add(addressBook);
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Changed address book " + addressBook.Id);
        }

        public List<Group> GetGroups()
        {
            var groups = _addressBooksCache.SelectMany(o => o.Groups).ToList();
            return groups;
        }

        public Group GetGroup(int id)
        {
            if (GetGroups().Find(o => o.Id == id) == null)
            {
                throw new Exception("Group not found");
            }
            return GetGroups().Find(o => o.Id == id);
        }

        public async Task<Group> CreateGroup(Group group)
        {
            group.Id = (await _api.CreateGroup(new RestGroup
            {
                AddressBook = @group.AddressBook.Id,
                Name = @group.Name,
                Addresses = @group.Addresses.Select(address => address.Id).ToList()
            }, "token " + _token)).Id;
            _addressBooksCache.Find(o => o.Id == group.AddressBook.Id).Groups.Add(group);
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Created group " + group.Id);
            return group;
        }

        public async Task DeleteGroup(Group group)
        {
            await _api.DeleteGroup(group.Id, "token " + _token);
            _addressBooksCache.Find(o => o.Id == group.AddressBook.Id).Groups.Remove(group);
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Deleted group " + group.Id);
        }

        public async Task ChangeGroup(Group group)
        {
            _addressBooksCache.Find(o => o.Id == group.AddressBook.Id).Groups.Remove(group);
            var restGroup = new RestGroup
            {
                Id = group.Id,
                Name = group.Name,
                AddressBook = group.AddressBook.Id,
                Addresses = group.Addresses.Select(o => o.Id).ToList()
            };
            await _api.ChangeGroup(group.Id, restGroup, "token " + _token);
            _addressBooksCache.Find(o => o.Id == group.AddressBook.Id).Groups.Add(group);
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Changed group " + group.Id);
        }

        public List<Address> GetAddresses()
        {
            List<Address> addresses = new List<Address>();
            foreach (Group group in GetGroups())
            {
                addresses.AddRange(group.Addresses);
            }
            return addresses;
        }

        public Address GetAddress(int id)
        {
            if (GetAddresses().Find(o => o.Id == id) == null)
            {
                throw new Exception("Address not found");
            }
            return GetAddresses().Find(o => o.Id == id);
        }

        public Address GetAddress(string email, int addressBookId)
        {
            return
                _addressBooksCache.Find(o => o.Id == addressBookId)
                    .Groups.SelectMany(o => o.Addresses)
                    .ToList()
                    .Find(o => o.Email == email);
        }

        public async Task<Address> CreateAddress(Address address)
        {
            address.Id = (await _api.CreateAddress(new RestAddress
            {
                Name = address.Name,
                Email = address.Email,
                GroupIds = address.Groups.Select(o => o.Id).ToList()
            }, "token " + _token)).Id;
            foreach (AddressBook addressBook in _addressBooksCache)
            {
                foreach (Group group in addressBook.Groups)
                {
                    if (address.Groups.Contains(group))
                    {
                        group.Addresses.Remove(address);
                        group.Addresses.Add(address);
                    }
                }
            }
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Created address " + address.Id);
            return address;
        }

        public async Task DeleteAddress(Address address)
        {
            await _api.DeleteAddress(address.Id, "token " + _token);
            _addressBooksCache.ForEach(a => a.Groups.ForEach(g => g.Addresses.Remove(g.Addresses.Find(o => o.Id == address.Id))));
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Deleted address " + address.Id);
        }

        public async Task ChangeAddress(Address address)
        {
            await _api.ChangeAddress(address.Id, new RestAddress
            {
                Id = address.Id,
                Name = address.Name,
                Email = address.Email,
                GroupIds = address.Groups.Select(o => o.Id).ToList()
            }, "token " + _token);

            _addressBooksCache.SelectMany(o => o.Groups).ToList().ForEach(o => o.Addresses.Remove(o.Addresses.Find(a => a.Id == address.Id)));

            foreach (var @group in address.Groups)
            {
                group.Addresses.Add(address);
            }
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
            Log("Changed address " + address.Id);
        }

        private async Task PopulateUsers()
        {
            _usersCache = new List<User>();

            RestUsersPage restUsersPage = await _api.GetUsers(1, "token " + _token);
            foreach (var user in restUsersPage.Results)
            {
                _usersCache.Add(user);
            }

            while (restUsersPage.Next != null)
            {
                foreach (var user in restUsersPage.Results)
                {
                    _usersCache.Add(user);
                }
                restUsersPage = await _api.GetUsers(GetNextPageNumberFromUrl(restUsersPage.Next), "token " + _token);
            }
        }

        private async Task PopulateAddressBooks()
        {
            _addressBooksCache = new List<AddressBook>();

            RestAddressBookPage addressBooksPage = await _api.GetAddressBooks(1, "token " + _token);
            foreach (RestAddressBook restAddressBook in addressBooksPage.Results)
            {
                var addressBook = new AddressBook
                {
                    Id = restAddressBook.Id,
                    Name = restAddressBook.Name,
                    Owner = GetUser(restAddressBook.OwnerId),
                    SharedWith = restAddressBook.SharedWithIds.Select(GetUser).ToList()
                };
                _addressBooksCache.Add(addressBook);
                addressBook = await PopulateAddressBookWithGroups(addressBook.Id);
                _addressBooksCache.Remove(_addressBooksCache.Find(o => o.Id == addressBook.Id));
                _addressBooksCache.Add(addressBook);
            }

            while (addressBooksPage.Next != null)
            {
                addressBooksPage = await _api.GetAddressBooks(GetNextPageNumberFromUrl(addressBooksPage.Next), "token " + _token);
                foreach (RestAddressBook restAddressBook in addressBooksPage.Results)
                {
                    var addressBook = new AddressBook
                    {
                        Id = restAddressBook.Id,
                        Name = restAddressBook.Name
                    };
                    _addressBooksCache.Add(addressBook);
                    addressBook = await PopulateAddressBookWithGroups(addressBook.Id);
                    _addressBooksCache.Remove(_addressBooksCache.Find(o => o.Id == addressBook.Id));
                    _addressBooksCache.Add(addressBook);
                }
            }
        }

        private async Task<AddressBook> PopulateAddressBookWithGroups(int addressBookId)
        {
            var addressBook = GetAddressBook(addressBookId);
            addressBook.Groups = new List<Group>();

            RestGroupsPage groupsPage = await _api.GetGroupsForAddressBook(addressBook.Id, 1, "token " + _token);
            foreach (RestGroup group in groupsPage.Results)
            {
                var newGroup = new Group
                {
                    Id = group.Id,
                    AddressBook = addressBook,
                    Name = group.Name
                };
                addressBook.Groups.Add(newGroup);
            }

            while (groupsPage.Next != null)
            {
                groupsPage = await _api.GetGroupsForAddressBook(addressBook.Id, GetNextPageNumberFromUrl(groupsPage.Next), "token " + _token);
                foreach (RestGroup group in groupsPage.Results)
                {
                    var newGroup = new Group
                    {
                        Id = group.Id,
                        AddressBook = addressBook,
                        Name = group.Name
                    };
                    addressBook.Groups.Add(newGroup);
                }
            }

            // Now that all groups have been added, populate them
            foreach (var @group in addressBook.Groups.ToList())
            {
                var newGroup = await PopulateGroupWithAddresses(group.Id);
                addressBook.Groups.Remove(newGroup);
                addressBook.Groups.Add(newGroup);
            }
            return addressBook;
        }

        private async Task<Group> PopulateGroupWithAddresses(int groupId)
        {
            var group = GetGroup(groupId);
            group.Addresses = new List<Address>();
            RestAddressesPage addressesPage = await _api.GetAddressesForGroup(group.Id, 1, "token " + _token);
            foreach (RestAddress address in addressesPage.Results)
            {
                var newAddress = new Address
                {
                    Id = address.Id,
                    Name = address.Name,
                    Groups = address.GroupIds.Select(GetGroup).ToList(),
                    Email = address.Email
                };
                group.Addresses.Add(newAddress);
            }

            while (addressesPage.Next != null)
            {
                addressesPage = await _api.GetAddressesForGroup(group.Id, GetNextPageNumberFromUrl(addressesPage.Next), "token " + _token);
                foreach (RestAddress address in addressesPage.Results)
                {
                    var newAddress = new Address
                    {
                        Id = address.Id,
                        Name = address.Name,
                        Groups = address.GroupIds.Select(GetGroup).ToList(),
                        Email = address.Email
                    };
                    group.Addresses.Add(newAddress);
                }
            }
            return group;
        }

        private int GetNextPageNumberFromUrl(string url)
        {
            return Convert.ToInt32(Regex.Match(url, @"[\?&]page=(\d+)$",
                RegexOptions.IgnoreCase).Groups[1].Value);
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public async Task<User> GetLoggedInUser()
        {
            var user = await _api.GetCurrentUser("token " + _token);
            user.Token = _token;
            return user;
        }

        public async void NotifyCanUpdate()
        {
            if (DateTime.Now.Subtract(_lastChange).TotalMinutes < 0.3) return;
            _lastChange = DateTime.Now;
            Log("Updating data...");
            await PopulateUsers();
            await PopulateAddressBooks();
            _registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
        }

        public void Register(IRegisterable registerable)
        {
            Console.WriteLine("Registered " + registerable);
            _registerables.Add(registerable);
        }

        public async Task Authenticate(string username, string password)
        {
            SetToken((await _api.CreateApiToken(new Dictionary<string, string>
            {
                {"username", username},
                {"password", password}
            }))["token"]);
        }

        public List<User> GetUsers()
        {
            return _usersCache;
        }

        public User GetUser(int id)
        {
            return _usersCache.Find(o => o.Id == id);
        }

        private void Log(string message)
        {
            Console.WriteLine("CachedAddressBooksApi: " + message);
        }
    }
}
