using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NetworkManager : MonoBehaviour 
{
	public GameObject sceneManager;
	private int size = 60;

	static string token = "";
	static string userId = "";

    // static string serverUri = "project06.codetalks.kr:9990";
	static string serverUri = "localhost:3579";
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



	
	public void LoginRequest( string id, string pw )
	{
		var data = new  Dictionary<string, object>();
		data.Add( "UserName", id );
		data.Add( "Password", pw );
		
		userId = id;
		
		StartCoroutine( WaitForLogin( POST( "/user/login", data ) ) );
	}
	
	private IEnumerator WaitForLogin( WWW www )
	{
		yield return www;
		
		// check for errors
		if (!RequestErrorHandling (www))
			yield break;
		
		// get json data as dictionary
		var dict =  JsonConvert.DeserializeObject<Dictionary<string, object>>( www.text );

		token = (string)dict["token"];

		Debug.Log ("Token : " + token);
		
		// check the result 
		// ... 
	}
	
	public void SignupRequest( string id, string pw, string name )
	{
		var data = new Dictionary<string, object>();
		data.Add( "UserId", id );
		data.Add( "password", pw );
		data.Add( "playerName", name );
		
		StartCoroutine( WaitForSignup( POST( "/user/signin", data ) ) );
	}
	
	private IEnumerator WaitForSignup(WWW www)
	{
		yield return www;

		// check for errors
		if (!RequestErrorHandling (www))
			yield break;
			
		if ( www.text.CompareTo ("success") == 0 )
			yield break;

		// display signup window
		// ...
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

		if ( www.text.CompareTo( "valid" ) == 0 )
			yield break;
		
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
			yield break;
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
		// var baseData =  JsonConvert.DeserializeObject<GameResultBaseData>( www.text );
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
		// var playerData =  JsonConvert.DeserializeObject<ClientPlayerData>( www.text );
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
		Debug.Log ("Add token : " + token);

		var headers = new Hashtable();
		headers.Add("Authorization", "Token " + token );

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
		
		// token!
		form.headers.Add( "Authorization", "Token " + token );
		
		WWW www = new WWW( serverUri + url, form );
		
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
		OperationBluehole.Content.Player[] player = { new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() };
		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 102 , out data ) )
			player[0].LoadPlayer( data );

		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 103 , out data ) )
			player[1].LoadPlayer( data );

		if ( OperationBluehole.Content.TestData.playerList.TryGetValue( 104 , out data ) )
			player[2].LoadPlayer( data );

		OperationBluehole.Content.Party DummyParty = new OperationBluehole.Content.Party( OperationBluehole.Content.PartyType.PLAYER , 10 );
		foreach ( OperationBluehole.Content.Player p in player )
			DummyParty.AddCharacter( p );

		int dummySize = 60;
		int dummySeed = 3;

		///////////////////////////////////////

		LogGenerator.Instance.GenerateLog( dummySize, dummySeed, DummyParty );
	}
	
}
