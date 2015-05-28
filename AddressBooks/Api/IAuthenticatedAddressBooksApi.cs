using System.Threading.Tasks;
using AddressBooks.Models;

namespace AddressBooks.Api
{
    interface IAuthenticatedAddressBooksApi : IAddressBooksApi
    {
        Task Authenticate(string username, string password);
        Task<User> GetLoggedInUser();
    }
}
