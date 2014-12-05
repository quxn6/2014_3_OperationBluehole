using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OperationBluhole.DummyClient
{
	class User
	{
		string userId, password;
		string token;

		public User(string userId, string password)
		{
			this.userId = userId;
			this.password = password;
		}

		public async void Start()
		{
			{
				bool chk = await Network.SignIn(this.userId, this.password, this.userId + "_1");
				Console.WriteLine("SignIn : {0}", chk);
			}

			{
				this.token = await Network.LogIn(userId, password);
				Console.WriteLine("Login Token : {0}", this.token);
			}

			{
				bool chk = await Network.IsLogIn(this.token);
				Console.WriteLine("Login Status : {0}", chk);
			}
		}

	}
}
