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
			bool chk = await Network.SignIn(this.userId, this.password, this.userId + "_1");

			this.token = await Network.LogIn(userId, password);
		}

	}
}
