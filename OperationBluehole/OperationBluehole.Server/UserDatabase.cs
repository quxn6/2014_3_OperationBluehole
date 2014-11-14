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
        // user name, password

        // user name, claims( List<string> ) - 일종의 권한
        // 특정 모듈 - 가령 관리자 모드 - 의 경우에는 claims 중에 admin 속성이 있어야 한다거나
        // demo 속성을 가지고 있어야 한다거나
        // 아무튼 그런 것들...

        // 위의 두 자료는 디비 안에 저장되어야 한다
        // 하나는 계정 로그인 정보 - id, password
        // 하나는 계정 속성 정보 - id, claims, character id
        // 캐릭터 정보 - 이건 이미 만들어 뒀고...
        
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
    }
}