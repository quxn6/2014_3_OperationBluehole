using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using OperationBluehole.Content;

public class NetworkManager : MonoBehaviour 
{
	internal class ClientPlayerData
	{
		public string Name { get; set; }
		
		public uint Exp { get; set; }
		public ushort StatPoints { get; set; }
		public List<ushort> Stat { get; set; }
		public List<ushort> Skill { get; set; }
		
		public uint Gold { get; set; }
		public List<uint> Inventory { get; set; }
		public List<ItemToken> Token { get; set; }
		
		public List<uint> Equipment { get; set; }
		public List<uint> Consumable { get; set; }
		
		public byte BattleStyle { get; set; }
		
		public List<string> BanList { get; set; }
		
		public ClientPlayerData()
		{
		}
	}
	
	internal class SimulationResult
	{
		public long Id { get; set; }
		
		// 참가한 player id 목록
		public List<PlayerData> PlayerList { get; set; }
		
		// 맵 크기
		public int MapSize { get; set; }
		
		// 게임 결과를 확인한 플레이어 - 전부 확인하면 지울 수 있도록? 아니면 아예 무조건 타임아웃? 적절히 혼합?
		public List<ulong> CheckedPlayer { get; set; }
		
		// 시뮬레이션에 사용한 random seed 값
		public int Seed { get; set; }
	}
	
	//public UnityEngine.GameObject sceneManager;
	
	static string token = "";
	
	// static string serverUri = "project06.codetalks.kr:9990";
	static string serverUri = "project06.codetalks.kr:3579";
    // client version check
    // ...

	static private NetworkManager instance;
	static public NetworkManager Instance
	{
		get { return instance; }	
	}

	void Awake()
	{
		instance = this;
	}

	public void SignupRequest( string id, string pw, string name )
	{
		Debug.Log( "signupcall by " + id );
		var data = new Dictionary<string, object>();
		data.Add( "UserId", id );
		data.Add( "password", pw );
		data.Add( "playerName", name );
		
		StartCoroutine( WaitForSignup( POST( "/user/signup", data ) ) );
	}
	
	private IEnumerator WaitForSignup(WWW www)
	{
		yield return www;

		// check for errors
		if (!RequestErrorHandling (www))
			yield break;

		Debug.Log( www.text );
		if (www.text.CompareTo ("success") == 0) 
		{
			Debug.Log("signup : success");
			//LoginRequest("quxn6","next!!@@##$$");
			yield break;
		}
		// display signup window
		// ...

		Debug.Log ("signup fail");
	}
	
	public void LoginRequest( string id, string pw )
	{
		var data = new  Dictionary<string, object>();
		data.Add( "UserName", id );
		data.Add( "Password", pw );
		
		StartCoroutine( WaitForLogin( POST( "/user/login", data ) ) );
	}
	
	private IEnumerator WaitForLogin( WWW www )
	{
		yield return www;
		
		// check for errors
		if (!RequestErrorHandling (www))
			yield break;
		
		// get json data as dictionary
		// var dict =  JsonConvert.DeserializeObject<Dictionary<string, object>>( www.text );
		var dict = JsonMapper.ToObject<Dictionary<string, object>>( www.text );
		
		token = (string)dict["token"];
		
		Debug.Log ("Token : " + token);
		
		// check the result 
		// ... 
		Application.LoadLevel( "MainMenu" );

		//SessionCheck ();
	}

	public void SessionCheck()
	{
		StartCoroutine( WaitForSessionCheck( GET( "/user/valid_session" ) ) );
	}
	
	private IEnumerator WaitForSessionCheck( WWW www )
	{
		yield return www;

		// check for errors
		if (!RequestErrorHandling (www)) 
			yield break;

		if (www.text.CompareTo ("valid") == 0) 
		{
			Debug.Log("valid session");
			//RegisterRequest(1);
			yield break;
		}
		// display login window
		// ...
	}
	
	public void RegisterRequest( int difficulty )
	{
		var data = new Dictionary<string, object>();
		data.Add( "difficulty", difficulty );
		
		StartCoroutine( WaitForRegister( POST( "/matching/register", data ) ) );
	}
	
	private IEnumerator WaitForRegister( WWW www )
	{
		yield return www;

		// check for errors
		if ( !RequestErrorHandling( www ) )
			yield break;
		
		if (www.text.CompareTo ("success") == 0)
		{
			Debug.Log("registered");
			//GetSimulationResult();
			yield break;
		}
		else if (www.text.CompareTo ("not prepared") == 0) 
		{
			// check the previous game result
			// ...
			
			yield break;
		}
	}
	
	public void GetSimulationResult()
	{
		StartCoroutine( WaitForSimulationResult( GET( "/character/simulation_result" ) ) );
	}
	
	private IEnumerator WaitForSimulationResult( WWW www )
	{
		yield return www;

		// check for errors
		if ( !RequestErrorHandling( www ) )
			yield break;
		
		// deserialize the base data
		if (www.text.CompareTo ("nothing") == 0)
			//GetSimulationResult ();
			//not yet

		//var baseData = JsonMapper.ToObject<SimulationResult>( www.text );
		Debug.Log (www.text);
	}
	
	public void GetPlayerInfo()
	{
		StartCoroutine( WaitForSimulationResult( GET( "/character/update" ) ) );
	}
	
	private IEnumerator WaitForPlayerInfo( WWW www )
	{
		yield return www;

		// check for errors
		if ( !RequestErrorHandling( www ) )
			yield break;
		
		// apply the current status

		//var playerData = JsonMapper.ToObject<ClientPlayerData>( www.text );
		Debug.Log (www.text);
	}
	
	public void LevelUpRequest()
	{
		StartCoroutine( WaitForLevelUp( GET( "/character/levelup" ) ) );
	}
	
	private IEnumerator WaitForLevelUp( WWW www )
	{
		yield return www;

		// check for errors
		if ( !RequestErrorHandling( www ) )
			yield break;
		
		// apply the changed level
		// ...
	}
	
	public void ChangeStatsRequest( int[] stats )
	{
		
	}
	
	private IEnumerator WaitForStatChange( WWW www )
	{
		yield return www;

		// check for errors
		if ( !RequestErrorHandling( www ) )
			yield break;
		
		// apply the changed stat
		// ...
	}


	private bool RequestErrorHandling( WWW www )
	{
		// check for errors 
		if ( www.error != null )
		{
			Debug.Log( www.error );

			if ( www.error.Contains("401 Unauthorized") )
			{
				Debug.Log("display login window");
				// ...
			}

			return false;
		}

		return true;
	}
	
	private WWW GET( string url )
	{
		var headers = new Dictionary<string,string>();
		headers.Add("Authorization", "Token " + token );

		Debug.Log (headers["Authorization"]);

		WWW www = new WWW( serverUri + url, null, headers );

		return www;
	}
	
	private WWW POST( string url, Dictionary<string, object> postData )
	{
		WWWForm form = new WWWForm();

		foreach (var each in postData) 
		{
			form.AddField( each.Key, each.Value.ToString() );
		}

		var headers = form.headers;
		var rawData = form.data;

		headers ["Authorization"] = "Token " + token;
		WWW www = new WWW(serverUri + url, rawData, headers);

		return www;
	}  

	// dungeon map ///
	public void RequestReplayInfo()
	{
		StartCoroutine( DummyMapInfoResponse() );
	}

	IEnumerator DummyMapInfoResponse()
	{
		yield return new WaitForSeconds( 0.1f );
		HandleReplayInfo();
	}

	public void HandleReplayInfo()
	{
		// Init
		OperationBluehole.Content.ContentsPrepare.Init();

		///////////// test data /////////////
		OperationBluehole.Content.PlayerData data = new OperationBluehole.Content.PlayerData();
		OperationBluehole.Content.Player[] player = { new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() };
		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 102 , out data ) )
			player[0].LoadPlayer( data );

		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 103 , out data ) )
			player[1].LoadPlayer( data );

		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 104 , out data ) )
			player[2].LoadPlayer( data );

		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 101 , out data ) )
			player[3].LoadPlayer( data );

		OperationBluehole.Content.Party DummyParty = new OperationBluehole.Content.Party( OperationBluehole.Content.PartyType.PLAYER , 3 );
		foreach ( OperationBluehole.Content.Player p in player )
			DummyParty.AddCharacter( p );

		int dummySize = 60;
		int dummySeed = 3;

		///////////////////////////////////////

		LogGenerator.Instance.GenerateLog( dummySize, dummySeed, DummyParty );
	}
	
}
