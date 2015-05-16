using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressBooks.Models;

namespace AddressBooks
{
    class AddressBooksApi
    {

        private static string Token;
        private static IAddressBookApi Api = RestService.For<IAddressBookApi>("http://addressbooks.favega.com/");
        private static List<AddressBook> AddressBookCache = new List<AddressBook>();

        public static DateTime LastUpdate;

        public static async Task PopulateApiToken(Dictionary<string, string> user)
        {
            Token = (await Api.CreateApiToken(user))["token"];
        }

        public static List<AddressBook> GetAddressBooks()
        {
            return AddressBookCache;
        }

        public static AddressBook GetAddressBook(int id)
        {
            return AddressBookCache.Find(x => x.Id == id);
        }

        public static void NotifyCanUpdate()
        {
            if (LastUpdate == null || DateTime.Now.Subtract(LastUpdate).TotalMinutes > 5)
            {
                Console.WriteLine("Updating data set...");
                LastUpdate = DateTime.Now;
                PopulateAddressBooks();
                Console.WriteLine("Done.");
            }
        }

        public static async void PopulateAddressBooks()
        {
            Console.WriteLine("Populating address books...");
            AddressBookCache = new List<AddressBook>();

            RestAddressBookPage addressBooksPage = await Api.GetAddressBooks(1, "token " + Token);
            foreach (AddressBook addressBook in addressBooksPage.Results){
                    AddressBookCache.Add(await PopulateAddressBookWithGroups(addressBook));
            }

            while (addressBooksPage.Next != null)
            {
                foreach (AddressBook addressBook in addressBooksPage.Results){
                    AddressBookCache.Add(await PopulateAddressBookWithGroups(addressBook));
                }
                addressBooksPage = await Api.GetAddressBooks(GetNextPageNumberFromUrl(addressBooksPage.Next), "token " + Token);
            }
        }

        private static async Task<AddressBook> PopulateAddressBookWithGroups(AddressBook addressBook)
        {
            addressBook.Groups = new List<Group>();

            RestGroupsPage groupsPage = await Api.GetGroupsForAddressBook(addressBook.Id, 1, "token " + Token);
            foreach (Group group in groupsPage.Results)
            {
                Console.WriteLine("Adding group to address book");
                addressBook.Groups.Add(await PopulateGroupWithAddresses(group));
            }

            while (groupsPage.Next != null)
            {
                foreach (Group group in groupsPage.Results)
                {
                    Console.WriteLine("Adding group to address book");
                    addressBook.Groups.Add(await PopulateGroupWithAddresses(group));
                }
                groupsPage = await Api.GetGroupsForAddressBook(addressBook.Id, GetNextPageNumberFromUrl(groupsPage.Next), "token " + Token);
            }
            return addressBook;
        }

        private static async Task<Group> PopulateGroupWithAddresses(Group group)
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

        private static int GetNextPageNumberFromUrl(string url)
        {
            return Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(url, @"\/\?page=(\d+)$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Groups[1].Value);
        }

        public static async Task<User> GetCurrentUser()
        {
            var user = await Api.GetCurrentUser("token " + Token);
            user.Token = Token;
            return user;
        }
    }
}
