using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Nancy.Security;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace OperationBluehole.Server
{
    public class UserDatabase
    {
        public static IUserIdentity ValidateUser(string userName, string password)
        {
            var client = CouchbaseManager.Client;

            var account = client.GetJson<AccountInfo>(AccountInfo.PREFIX + userName);

            if (account == null || !account.Password.Equals(password))
                return null;

            var userIdentity = client.GetJson<UserIdentity>(UserIdentity.PREFIX + userName);
            var claims = userIdentity.Claims;

            return new UserIdentity { UserName = account.AccountName, Claims = claims };
        }

        // Nancy module에서 token을 사용할 때
        // context의 UserInterface에 포함되는 내용은 Nancy에서 제공하는 IUserIdentity를 따른다
        // 그래서 context로 접근해서는 palyerId를 얻을 수 없음
        // 방법은 claim에 playerId를 끼워 넣는 것과
        // 아니면 userName과 playerData의 key값을 같은 걸로 사용 - userName을 이용해서 검색 - 하는 방법 두 가지
        // 일단은 후자로 구현하지만... 구리다...매우 구리다
    }
}