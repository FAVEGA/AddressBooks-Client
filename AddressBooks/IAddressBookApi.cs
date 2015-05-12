﻿using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBooks
{
    public interface IAddressBookApi
    {
        [Post("/api-token-auth/")]
        Task<Dictionary<string, string>> CreateApiToken([Body] Dictionary<string, string> user);

        [Get("/whoami/")]
        Task<User> GetCurrentUser([Header("Authorization")] string authorization);

        [Get("/addressbooks/?page={page}")]
        Task<RestAddressBookPage> GetAddressBooks(int page, [Header("Authorization")] string authorization);

        [Get("/addressbooks/{id}")]
        Task<AddressBook> GetAddressBook(int id, [Header("Authorization")] string authorization);

        [Get("/groups/?page={page}")]
        Task<List<Group>> GetGroups(int page, [Header("Authorization")] string authorization);

        [Get("/addressbooks/{id}")]
        Task<Group> GetGroup(int id, [Header("Authorization")] string authorization);
    }
}