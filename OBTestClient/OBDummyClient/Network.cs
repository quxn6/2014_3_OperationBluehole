﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Cache;

namespace OperationBluhole.DummyClient
{
	static class Network
	{
		static Uri rootUri;
		static HttpRequestCachePolicy cachePolicy;
		static string method;
		static string contentType;

		public static bool Init(string rootUrl)
		{
			rootUri = new Uri(rootUrl);
			cachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
			contentType = "application/x-www-form-urlencoded";
			method = "POST";

			return true;
		}

		static async Task<string> SendRequest(string reqUrl, string postData, string cookie)
		{
			byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);

			var wRequest = (HttpWebRequest)WebRequest.Create(rootUri + reqUrl);
			wRequest.Method = method;
			wRequest.ContentLength = postData.Length;
			wRequest.ContentType = contentType;
			wRequest.CachePolicy = cachePolicy;
			wRequest.CookieContainer = new CookieContainer();
			wRequest.CookieContainer.SetCookies(rootUri, cookie);

			Stream requestStream = await wRequest.GetRequestStreamAsync();
			requestStream.Write(sendData, 0, sendData.Length);
			

			try
			{
				HttpWebResponse wResponse = (HttpWebResponse)await wRequest.GetResponseAsync();

				string res;
				if (wResponse.StatusCode == HttpStatusCode.OK)
				{
					StreamReader streamReader = new StreamReader(wResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
					res = await streamReader.ReadToEndAsync();
					streamReader.Close();
				}
				else
					return "";

				requestStream.Close();
				wResponse.Close();

				return res;
			}
			catch (Exception e)
			{
				requestStream.Close();
				return "";
			}
		}

		public static async Task<bool> SignIn(string userId, string pw, string playerName)
		{
			var postData = String.Format("userId={0}&password={1}&playername={2}", userId, pw, playerName);

			var res = await SendRequest("/user/signin", postData, "");

			return (res == "success");
		}

		public static async Task<string> LogIn(string userId, string pw)
		{
			var postData = String.Format("UserName={0}&Password={1}", userId, pw);

			var res = await SendRequest("/user/login", postData, "");

			if( res != "")
				res = "token=" + Json.JsonParser.Deserialize(res).token;

			return res;
		}

		public static async Task<bool> IsLogIn(string token)
		{
			var res = await SendRequest("/user/valid_session", "", token);

			return (res == "valid");
		}

		public static async Task<string> GetPlayerData(string token)
		{
			var res = await SendRequest("/character/update", "", token);

			return res;
		}

		public static async Task<string> LevelUp(string token)
		{
			var res = await SendRequest("/character/levelup", "", token);

			return res;
		}

		public static async Task<string> IncreaseStats(string token, ushort[] stats)
		{
			var postData = String.Format("stat={0}&stat={1}&stat={2}&stat={3}&stat={4}&stat={5}", stats[0], stats[1], stats[2], stats[3], stats[4], stats[5]);

			var res = await SendRequest("/character/increase_stat", postData, token);

			return res;
		}

		public static async Task<string> GetSimulationResult(string token)
		{
			var res = await SendRequest("/character/simulation_result", "", token);

			return res;
		}
	}
}
