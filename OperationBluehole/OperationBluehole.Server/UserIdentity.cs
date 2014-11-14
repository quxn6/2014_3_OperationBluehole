using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Nancy.Security;
using Newtonsoft.Json;

namespace OperationBluehole.Server
{
    public class AccountInfo
    {
        public const string PREFIX = "Account ";

        [JsonProperty("name")]
        public string AccountName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public AccountInfo(string name, string password)
        {
            this.AccountName = name;
            this.Password = password;
        }
    }

    public class UserIdentity : IUserIdentity
    {
        public const string PREFIX = "UserIdentity ";

        [JsonProperty("name")]
        public string UserName { get; set; }

        [JsonProperty("claims")]
        public IEnumerable<string> Claims { get; set; }
    }
}