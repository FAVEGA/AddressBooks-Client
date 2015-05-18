using AddressBooks.Models;
using AddressBooks.Rest;
using AddressBooks.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.Api
{
    class CachedAddressBooksApi : IAddressBooksApi, IRegistrar
    {

        private string Token;
        private IRestAddressBookApi Api = RestService.For<IRestAddressBookApi>("http://addressbooks.favega.com/");
        private List<AddressBook> Cache = new List<AddressBook>();
        private DateTime LastUpdate;
        private List<IRegisterable> Registerables = new List<IRegisterable>();

        public List<AddressBook> GetAddressBooks()
        {
            return Cache;
        }

        public AddressBook GetAddressBook(int id)
        {
            return Cache.Find(o => o.Id == id);
        }

        public void CreateAddressBook(AddressBook addressBook)
        {
            throw new NotImplementedException();
        }

        public void DeleteAddressBook(AddressBook addressBook)
        {
            throw new NotImplementedException();
        }

        public void UpdateAddressBook(AddressBook addressBook)
        {
            throw new NotImplementedException();
        }

        public List<Group> GetGroups()
        {
            List<Group> groups = new List<Group>();
            foreach (AddressBook addressBook in Cache)
            {
                groups.AddRange(addressBook.Groups);
            }
            return groups;
        }

        public Group GetGroup(int id)
        {
            return this.GetGroups().Find(o => o.Id == id);
        }

        public void CreateGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public void UpdateGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public List<Address> GetAddresses()
        {
            List<Address> addresses = new List<Address>();
            foreach (Group group in this.GetGroups())
            {
                addresses.AddRange(group.Addresses);
            }
            return addresses;
        }

        public Address GetAddress(int id)
        {
            return this.GetAddresses().Find(o => o.Id == id);
        }

        public void CreateAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public void DeleteAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public void UpdateAddress(Address address)
        {
            throw new NotImplementedException();
        }

        private async void PopulateAddressBooks()
        {
            Console.WriteLine("Populating address books...");
            Cache = new List<AddressBook>();

            RestAddressBookPage addressBooksPage = await Api.GetAddressBooks(1, "token " + Token);
            foreach (AddressBook addressBook in addressBooksPage.Results)
            {
                Cache.Add(await PopulateAddressBookWithGroups(addressBook));
            }

            while (addressBooksPage.Next != null)
            {
                foreach (AddressBook addressBook in addressBooksPage.Results)
                {
                    Cache.Add(await PopulateAddressBookWithGroups(addressBook));
                }
                addressBooksPage = await Api.GetAddressBooks(GetNextPageNumberFromUrl(addressBooksPage.Next), "token " + Token);
            }
        }

        private async Task<AddressBook> PopulateAddressBookWithGroups(AddressBook addressBook)
        {
            addressBook.Groups = new List<Group>();

            RestGroupsPage groupsPage = await Api.GetGroupsForAddressBook(addressBook.Id, 1, "token " + Token);
            foreach (Group group in groupsPage.Results)
            {
                addressBook.Groups.Add(await PopulateGroupWithAddresses(group));
            }

            while (groupsPage.Next != null)
            {
                foreach (Group group in groupsPage.Results)
                {
                    addressBook.Groups.Add(await PopulateGroupWithAddresses(group));
                }
                groupsPage = await Api.GetGroupsForAddressBook(addressBook.Id, GetNextPageNumberFromUrl(groupsPage.Next), "token " + Token);
            }
            return addressBook;
        }

        private async Task<Group> PopulateGroupWithAddresses(Group group)
        {
            group.Addresses = new List<Address>();
            RestAddressesPage addressesPage = await Api.GetAddressesForGroup(group.Id, 1, "token " + Token);
            foreach (Address address in addressesPage.Results)
            {
                group.Addresses.Add(address);
            }

            while (addressesPage.Next != null)
            {
                foreach (Address address in addressesPage.Results)
                {
                    group.Addresses.Add(address);
                }
                addressesPage = await Api.GetAddressesForGroup(group.Id, GetNextPageNumberFromUrl(addressesPage.Next), "token " + Token);
            }
            return group;
        }

        private int GetNextPageNumberFromUrl(string url)
        {
            return Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(url, @"\/\?page=(\d+)$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Groups[1].Value);
        }

        public async Task PopulateApiToken(Dictionary<string, string> user)
        {
            this.Token = (await Api.CreateApiToken(user))["token"];
        }

        public async Task<User> GetCurrentUser()
        {
            var user = await Api.GetCurrentUser("token " + this.Token);
            user.Token = this.Token;
            return user;
        }

        public void NotifyCanUpdate()
        {
            if (LastUpdate == null || DateTime.Now.Subtract(LastUpdate).TotalMinutes > 5)
            {
                Console.WriteLine("Updating data set...");
                this.LastUpdate = DateTime.Now;
                this.PopulateAddressBooks();
                Registerables.ForEach(registerable => registerable.NotifyDataSetChanged());
                Console.WriteLine("Done.");
            }
        }

        public void Register(IRegisterable registerable)
        {
            this.Registerables.Add(registerable);
        }
    }
}
