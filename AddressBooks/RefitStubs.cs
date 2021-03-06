﻿﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressBooks.Models;
using Refit;

/* ******** Hey You! *********
 *
 * This is a generated file, and gets rewritten every time you build the
 * project. If you want to edit it, you need to edit the mustache template
 * in the Refit package */

namespace RefitInternalGenerated
{
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
    sealed class PreserveAttribute : Attribute
    {
#pragma warning disable 0649
        //
        // Fields
        //
        public bool AllMembers;

        public bool Conditional;
#pragma warning restore 0649
    }
}

namespace AddressBooks.Rest
{
    using RefitInternalGenerated;

    [Preserve]
    public partial class AutoGeneratedIRestAddressBookApi : IRestAddressBookApi
    {
        public HttpClient Client { get; protected set; }
        readonly Dictionary<string, Func<HttpClient, object[], object>> methodImpls;

        public AutoGeneratedIRestAddressBookApi(HttpClient client, IRequestBuilder requestBuilder)
        {
            methodImpls = requestBuilder.InterfaceHttpMethods.ToDictionary(k => k, v => requestBuilder.BuildRestResultFuncForMethod(v));
            Client = client;
        }

        public virtual Task<Dictionary<string, string>> CreateApiToken(Dictionary<string, string> user)
        {
            var arguments = new object[] { user };
            return (Task<Dictionary<string, string>>) methodImpls["CreateApiToken"](Client, arguments);
        }

        public virtual Task<User> GetCurrentUser(string authorization)
        {
            var arguments = new object[] { authorization };
            return (Task<User>) methodImpls["GetCurrentUser"](Client, arguments);
        }

        public virtual Task<RestAddressBookPage> GetAddressBooks(int page,string authorization)
        {
            var arguments = new object[] { page,authorization };
            return (Task<RestAddressBookPage>) methodImpls["GetAddressBooks"](Client, arguments);
        }

        public virtual Task<RestAddressBook> CreateAddressBook(RestAddressBook addressBook,string authorization)
        {
            var arguments = new object[] { addressBook,authorization };
            return (Task<RestAddressBook>) methodImpls["CreateAddressBook"](Client, arguments);
        }

        public virtual Task ChangeAddressBook(int id,RestAddressBook addressBook,string authorization)
        {
            var arguments = new object[] { id,addressBook,authorization };
            return (Task) methodImpls["ChangeAddressBook"](Client, arguments);
        }

        public virtual Task DeleteAddressBook(int id,string authorization)
        {
            var arguments = new object[] { id,authorization };
            return (Task) methodImpls["DeleteAddressBook"](Client, arguments);
        }

        public virtual Task<RestAddressBook> GetAddressBook(int id,string authorization)
        {
            var arguments = new object[] { id,authorization };
            return (Task<RestAddressBook>) methodImpls["GetAddressBook"](Client, arguments);
        }

        public virtual Task<RestGroupsPage> GetGroups(int page,string authorization)
        {
            var arguments = new object[] { page,authorization };
            return (Task<RestGroupsPage>) methodImpls["GetGroups"](Client, arguments);
        }

        public virtual Task<RestGroupsPage> GetGroupsForAddressBook(int addressBook,int page,string authorization)
        {
            var arguments = new object[] { addressBook,page,authorization };
            return (Task<RestGroupsPage>) methodImpls["GetGroupsForAddressBook"](Client, arguments);
        }

        public virtual Task<RestGroup> CreateGroup(RestGroup restGroup,string authorization)
        {
            var arguments = new object[] { restGroup,authorization };
            return (Task<RestGroup>) methodImpls["CreateGroup"](Client, arguments);
        }

        public virtual Task<RestGroup> GetGroup(int id,string authorization)
        {
            var arguments = new object[] { id,authorization };
            return (Task<RestGroup>) methodImpls["GetGroup"](Client, arguments);
        }

        public virtual Task<RestGroup> ChangeGroup(int id,RestGroup restGroup,string authorization)
        {
            var arguments = new object[] { id,restGroup,authorization };
            return (Task<RestGroup>) methodImpls["ChangeGroup"](Client, arguments);
        }

        public virtual Task<RestGroup> DeleteGroup(int id,string authorization)
        {
            var arguments = new object[] { id,authorization };
            return (Task<RestGroup>) methodImpls["DeleteGroup"](Client, arguments);
        }

        public virtual Task<RestAddress> CreateAddress(RestAddress address,string authorization)
        {
            var arguments = new object[] { address,authorization };
            return (Task<RestAddress>) methodImpls["CreateAddress"](Client, arguments);
        }

        public virtual Task<RestAddressesPage> GetAddressesForGroup(int group,int page,string authorization)
        {
            var arguments = new object[] { group,page,authorization };
            return (Task<RestAddressesPage>) methodImpls["GetAddressesForGroup"](Client, arguments);
        }

        public virtual Task DeleteAddress(int id,string authorization)
        {
            var arguments = new object[] { id,authorization };
            return (Task) methodImpls["DeleteAddress"](Client, arguments);
        }

        public virtual Task ChangeAddress(int id,RestAddress address,string authorization)
        {
            var arguments = new object[] { id,address,authorization };
            return (Task) methodImpls["ChangeAddress"](Client, arguments);
        }

        public virtual Task<RestUsersPage> GetUsers(int page,string authorization)
        {
            var arguments = new object[] { page,authorization };
            return (Task<RestUsersPage>) methodImpls["GetUsers"](Client, arguments);
        }

        public virtual Task<User> Getuser(int id,string authorization)
        {
            var arguments = new object[] { id,authorization };
            return (Task<User>) methodImpls["Getuser"](Client, arguments);
        }

    }
}


