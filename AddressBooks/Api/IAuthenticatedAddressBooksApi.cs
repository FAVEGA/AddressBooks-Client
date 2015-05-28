using AddressBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks.Api
{
    interface IAuthenticatedAddressBooksApi : IAddressBooksApi
    {
        Task Authenticate(string username, string password);
        Task<User> GetLoggedInUser();
    }
}
