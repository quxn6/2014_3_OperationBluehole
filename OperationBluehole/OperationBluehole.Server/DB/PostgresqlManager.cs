using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace OperationBluehole.Server
{
    using Nancy.Security;

    public class AccountData
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string[] Claims { get; set; }

        public AccountData(string name, string password, string[] claims)
        {
            this.AccountName = name;
            this.Password = password;
            this.Claims = claims;
        }
    }

    public class UserIdentity : IUserIdentity
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }

    public static class PostgresqlManager
	{
        const string Connection_String = 
            "Server=" + Config.POSTGRESQL_SERVER + ";" +
            "Port=" + Config.POSTGRESQL_PORT + ";" +
            "User Id=" + Config.POSTGRESQL_ID + ";" +
            "Password=" + Config.POSTGRESQL_PW + ";" +
            "Database=" + Config.POSTGRESQL_TARGET_DB + ";";

        const string Query_SetAccountInfo = "INSERT INTO accounts ( id, password, claims ) VALUES ( :id, :password, :claims );";
        const string Query_GetAccountInfo = "SELECT * FROM accounts WHERE id=:id LIMIT 1;";

        static readonly string[] userIdentity = { "user" };

        public static async Task<bool> SetAccountInfo(string userId, string password)
        {
            var conn = new Npgsql.NpgsqlConnection(Connection_String);
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(Query_SetAccountInfo, conn);
                cmd.Parameters.Add("id", NpgsqlTypes.NpgsqlDbType.Varchar, 16);
                cmd.Parameters[0].Value = userId;
                cmd.Parameters.Add("password", NpgsqlTypes.NpgsqlDbType.Text);
                cmd.Parameters[1].Value = password;
                cmd.Parameters.Add("claims", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Varchar);
                cmd.Parameters[2].Value = userIdentity;
                conn.Open();
                int rowAffected = await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return rowAffected > 0;
            }
            catch
            {
                conn.Close();
                return false;
            }
        }

        public static async Task<AccountData> GetAccountInfo(string userId)
        {
            var conn = new Npgsql.NpgsqlConnection(Connection_String);
            NpgsqlCommand cmd = new NpgsqlCommand(Query_GetAccountInfo, conn);
            cmd.Parameters.Add("id", NpgsqlTypes.NpgsqlDbType.Varchar, 16);
            cmd.Parameters[0].Value = userId;
            conn.Open();
            var res = await cmd.ExecuteReaderAsync();

            if (await res.ReadAsync())
            {
                var accountData = new AccountData((string)res[1], (string)res[2], (string[])res[3]);
                res.Close();
                conn.Close();
                return accountData;
            }
            else
            {
                res.Close();
                conn.Close();
                return null;
            }
        }

        public static async Task<UserIdentity> ValidateUser(string userId, string password)
        {
            var accountData = await GetAccountInfo(userId);

            if (accountData == null || !accountData.Password.Equals(password))
                return null;

            return new UserIdentity { UserName = accountData.AccountName, Claims = accountData.Claims };
        }
	}
}
