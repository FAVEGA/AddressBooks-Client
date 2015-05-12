using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddressBooks
{
    class RestApi
    {

        private static string Token;
        private static IAddressBookApi Api = RestService.For<IAddressBookApi>("http://addressbooks.favega.com/");

        public static async Task PopulateApiToken(Dictionary<string, string> user)
        {
            Token = (await Api.CreateApiToken(user))["token"];
        }

        private static Dictionary<AddressBook, List<Group>> AddressBookGroupCache = new Dictionary<AddressBook, List<Group>>();
        private static async void PopulateAddressBookGroups(AddressBook addressBook)
        {
            if (!AddressBookGroupCache.ContainsKey(addressBook) || !AddressBookGroupCache[addressBook].Any())
            {
                Console.Write("Cache miss!\n");
                List<Group> groups = new List<Group>();
                foreach (int group_id in addressBook.GroupIds)
                {
                    var group = await RestApi.GetGroup(group_id);
                    groups.Add(group);
                }
                AddressBookGroupCache[addressBook] = groups;
            }
        }


        public static async Task<AddressBook> GetAddressBook(int id)
        {
            var addressBook = await Api.GetAddressBook(id, "token " + Token);
            addressBook.Groups = AddressBookGroupCache[addressBook];
            return addressBook;
        }

        public static async Task<List<AddressBook>> GetAddressBooks()
        {
            var addressBooks = new List<AddressBook>();
            Console.WriteLine("Using token " + Token);
            RestAddressBookPage page = await Api.GetAddressBooks(1, "token " + Token);
            while (page.Next != null)
            {
                addressBooks.AddRange(page.Results);
                Console.WriteLine(Regex.Match(page.Next, @"\/\?page=(\d+)$", RegexOptions.IgnoreCase).Groups[1].Value);
                int nextPage = Convert.ToInt32(Regex.Match(page.Next, @"\/\?page=(\d+)$", RegexOptions.IgnoreCase).Groups[1].Value);
                Console.WriteLine("Next page: " + nextPage);
                page = await Api.GetAddressBooks(nextPage, "token " + Token);
            }
            return addressBooks;
        }

        public static async Task<Group> GetGroup(int id)
        {
            return await Api.GetGroup(id, "token " + Token);
        }

        public static async Task<User> GetCurrentUser()
        {
            var user = await Api.GetCurrentUser("token " + Token);
            user.Token = Token;
            return user;
        }
    }
}
