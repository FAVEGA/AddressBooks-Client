using Newtonsoft.Json;

namespace AddressBooks.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("email")]
        public string Email;

        [JsonIgnore]
        public string Token;

        [JsonProperty("can_add_addressbook")]
        public bool CanAddAddressBook;

        [JsonProperty("can_change_addressbook")]
        public bool CanChangeAddressBook;

        [JsonProperty("can_delete_addressbook")]
        public bool CanDeleteAddressBook;

        [JsonProperty("can_add_group")]
        public bool CanAddGroup;

        [JsonProperty("can_change_group")]
        public bool CanChangeGroup;

        [JsonProperty("can_delete_group")]
        public bool CanDeleteGroup;

        [JsonProperty("can_add_address")]
        public bool CanAddAddress;

        [JsonProperty("can_change_address")]
        public bool CanChangeAddress;

        [JsonProperty("can_delete_address")]
        public bool CanDeleteAddress;

        [JsonProperty("can_share_addressbook")]
        public bool CanShareAddressBook;

        [JsonProperty("can_assign_permissions")]
        public bool CanAssignPermissions;

        public override string ToString()
        {
            return Username;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is User)
            {
                return ((User)obj).Id == Id;
            }
            return base.Equals(obj);
        }
    }
}
