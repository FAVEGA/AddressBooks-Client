using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBooks.Models;
using Refit;

namespace AddressBooks.Rest
{
    public interface IRestAddressBookApi
    {
        [Post("/api-token-auth/")]
        Task<Dictionary<string, string>> CreateApiToken([Body] Dictionary<string, string> user);

        [Get("/whoami/")]
        Task<User> GetCurrentUser([Header("Authorization")] string authorization);

        [Get("/addressbooks/?page={page}")]
        Task<RestAddressBookPage> GetAddressBooks(int page, [Header("Authorization")] string authorization);

        [Post("/addressbooks/")]
        Task<RestAddressBook> CreateAddressBook([Body] RestAddressBook addressBook, [Header("Authorization")] string authorization);

        [Put("/addressbooks/{id}")]
        Task ChangeAddressBook(int id, [Body] RestAddressBook addressBook, [Header("Authorization")] string authorization);

        [Delete("/addressbooks/{id}")]
        Task DeleteAddressBook(int id, [Header("Authorization")] string authorization);

        [Get("/addressbooks/{id}")]
        Task<RestAddressBook> GetAddressBook(int id, [Header("Authorization")] string authorization);

        [Get("/groups/?page={page}")]
        Task<RestGroupsPage> GetGroups(int page, [Header("Authorization")] string authorization);

        [Get("/groups/?addressbook={addressBook}&page={page}")]
        Task<RestGroupsPage> GetGroupsForAddressBook(int addressBook, int page, [Header("Authorization")] string authorization);

        [Post("/groups/")]
        Task<RestGroup> CreateGroup([Body] RestGroup restGroup, [Header("Authorization")] string authorization);

        [Get("/groups/{id}")]
        Task<RestGroup> GetGroup(int id, [Header("Authorization")] string authorization);

        [Put("/groups/{id}")]
        Task<RestGroup> ChangeGroup(int id, [Body] RestGroup restGroup, [Header("Authorization")] string authorization);

        [Delete("/groups/{id}")]
        Task<RestGroup> DeleteGroup(int id, [Header("Authorization")] string authorization);

        [Post("/addresses/")]
        Task<RestAddress> CreateAddress([Body] RestAddress address, [Header("Authorization")] string authorization);

        [Get("/addresses/?group={group}&page={page}")]
        Task<RestAddressesPage> GetAddressesForGroup(int group, int page, [Header("Authorization")] string authorization);

        [Delete("/addresses/{id}")]
        Task DeleteAddress(int id, [Header("Authorization")] string authorization);

        [Put("/addresses/{id}")]
        Task ChangeAddress(int id, [Body] RestAddress address, [Header("Authorization")] string authorization);

        [Get("/users/?page={page}")]
        Task<RestUsersPage> GetUsers(int page, [Header("Authorization")] string authorization);

        [Get("/users/{id}")]
        Task<User> Getuser(int id, [Header("Authorization")] string authorization);
    }
}
