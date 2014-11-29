using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
	public GameObject sceneManager;

	// warning!!! move it to "Datamanager" class
	//private string dungeonMap = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOOOOOOOOOOOOOOOOOOOOOOOOOOOOOXI               XX     I  M  XOXXXXXXXXXXXXXOOXXXXXXXXXXXXOXXXXX XXXXXXXXXXXXX           XOX           XOOX          XOXXXXX XXXXXXXXXXXXXXXXX XXXXXXXOXXXXX XXXXXXXOOXXXXXXXX XXXOX                       XOOOOOOOOOOOX XOOOOOOOOOOOOOOOX XOOOXI        I      XXXXXX XXXXXXOXXXXXX XXXXXXXXXXXOOOOOX XOOOX    MMM  I      XOX         XOX                 XXXXXX XXXOX                XOXX XXXXXXXXOX             XXXI     M   XOXXXXXXXXXXXXXXXXXXOOX XOOOOOOOOX        II M XOX          XOOOOOOOOOOOOOOOOOOXXXX XXXXXXXXXX             XOXXXXXXXXXXXXOOXXXXXXXXXXXXXXXOX          I XXXXX XXXXXXXXXXOOOOOOOOOOOOOOOX   M      I  XXX        M   XXXXX XXXXXXXXXXXXXXXXXXXXXXXXOX                I M         XXI        XX    M  I        XOXXXXXXXXXXXX XXXX            XX         XX                XOOOOOOOOOOOOX XOOXXXXXXXX XXXXXX      M  XXXXXXXXXXXXXXXX XXXXXXXXXXXXXXX XXXXXXXXXXX XXX               XXXXXXXXXXXXX XXXI         I    XX            XXXXXX XXXXXX      I         XX    M          XX   M        XOOOOX XOOOOX     M       M  XX               XX  M    I    XOXXXX XXXXOXI           M   XX         I  M  XX    I I     XX        XOXI        I    M XX               XX            XX XXXXXXXXOX                XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX XOOOOOOOOXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX XXXXXXXXXXXXXXXXXXXXXXXXXXXX            P   XX       I         XX      XX        I    XX    I   I  M        I              XX   M  XX  I   M      XX                XX     M   I       XX      XX       M     XXXXXXXXXX XXXXXXXXXM   I      M     XX      XX             XXXXXXXXXX XXXXXXXXX   I             XX  I   XX I      M    XX          I M   XXM                XX      XX             XX            MI  XX                 XX XXXXXXXXXXXXX XXXXXXXX   MI      M    XXXXXXXXX XXXXXXXXXXX XXXXXXOOOOOOX XOOOOOOX       I I   I  XOOOOOOOX XOOOOOOOOOX    I XOXXXXXX XXXXXXOX   M            XOXXXXXXX XXXXXXXXXOXI     XOXI          XOX      M         XOX  M         I  XOXM     XOX    M      XOX                XOX               XOX      XOX M         XOX                XOXXXXXXXXXXXXXXXXXOX      XOX        I  XOXXXXXXXXXXXXX XXXXOOOOOOOOOOOOOOOOOOOX   I  XXX I  I      XOOOOOOOOOOOOOX XOOOOOOOOOOOOOOOOOOOOOOX                    XOOXXXXXXXXXXXX XXXXXXXXXXXXXXXXXXXXXXOX      XXXI          XOOX         XX X     M              XOX      XOX           XOOX I       XX X                    XOXM     XOX      I    XOOX M I     XX X                    XOX      XOX           XOOX         XX X       I I          XOX  IM  XOX  M     M  XOOX         XX X  M   I  I      M   XOX M    XOX           XOOX I M              M        MI    XOX      XOXXXXXXXXXXXXXOOX       I XXXX       M     I      XOXXXXX XXOOOOOOOOOOOOOOOOX         XOOX M     I   MI   I   XOXXXXX XXXXXXXXXXXXXXXXXOX     M   XOOX                    XOX  I     XX           XOX         XOOXXXXXXXXXXXXXXX XXXXXXXX  M     XX I       M XOXXXXXXX XXXOOOOOOOOOOOOOOOOX                 XX           XOOOOOOOX XOOOXXXXXXXXXXXXXXXX XXXXXXXXXXXX XXXXXXXXX XXXXXXXXXXXXXXX XXXXX               M      XXXXXX XX    XXX XXXXXXXX        M  XX   M I         M I    XX     I  XX   M  I    XX      M    XX      I        M      XX        XX        I  XX    M      XX        I             XX    M   XXM          XXI        I XX          M           XX  I     XX M         XX    I      XXI                I    XX        XX           XX           XX        I    MM  I    XX        XX   I       XX           XX                      XX        XX           XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
	private int size = 60;

    static string token = "";
    static string userId = "";

    static string serverUri = "project06.codetalks.kr:9990";
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

    /*
    void HandleToken()
    {
        // token token
    }

    void LoginRequest( string id, string pw )
    {
        var data = new Dictionary<string, string>();
        data.Add( "UserName", id );
        data.Add( "Password", pw );

        userId = id;

        // StartCoroutine( WaitForLogin( POST( "/auth", Json.Serialize( data ) ) ) ); 
        StartCoroutine( WaitForLogin( POST( "/user/login", data ) ) ); 
    }

    private IEnumerator WaitForLogin( WWW www )
    {
        yield return www;

        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        // get json data as dictionary
        var dict = Json.Deserialize( www.text ) as Dictionary<string, object>;

        char[] splitter = { ' ' };
        token = (string)dict["token"];

        // check the result 
        // ... 

        token = tokenResult[1];
    }

    void SignupRequest( string id, string pw, string name )
    {
        var data = new Dictionary<string, string>();
        data.Add( "UserId", id );
        data.Add( "password", pw );
        data.Add( "playerName", name );

        StartCoroutine( WaitForSignup( POST( "/user/signin", data ) ) );
    }

    private IEnumerator WaitForSignup(WWW www)
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        if ( www.text.CompareTo( "success" ) == 0 )
            return true;

        return false;
    }

    void SessionCheck()
    {
        StartCoroutine( WaitForSignup( GET( "/user/valid_session" ) ) );
    }

    private IEnumerable WaitForSessionCheck( WWW wwww )
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        if ( www.text.CompareTo( "valid" ) == 0 )
            return true;

        return false;
    }

    void RegisterRequest( int difficulty )
    {
        var data = new Dictionary<string, string>();
        data.Add( "difficulty", difficulty );

        StartCoroutine( WaitForRegister( POST( "/matching/register", data ) ) );
    }

    private IEnumerable WaitForRegister( WWW www )
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        if ( www.text.CompareTo( "success" ) == 0 )
            return true;
        else if ( www.text.CompareTo( "not prepared" ) == 0 )
            return false;   // check the previous game result

        return false;
    }

    void GetSimulationResult()
    {
        StartCoroutine( WaitForSimulationResult( GET( "/character/simulation_result" ) ) );
    }

    private IEnumerator WaitForSimulationResult( WWW www )
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        // deserialize the base data
        // ...
    }

    void GetPlayerInfo()
    {
        StartCoroutine( WaitForSimulationResult( GET( "/character/update" ) ) );
    }

    private IEnumerator WaitForPlayerInfo( WWW www )
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        // apply the current status
        // ...
    }

    void LevelUpRequest()
    {
        StartCoroutine( WaitForLevelUp( GET( "/character/levelup" ) ) );
    }

    private IEnumerable WaitForLevelUp( WWW www )
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        // apply the changed level
        // ...
    }

    void ChangeStatsRequest( int[] stats )
    {

    }

    private IEnumerable WaitForStatChange( WWW www )
    {
        // check for errors
        if ( !RequestErrorHandling( www ) )
            yield return false;

        // apply the changed stat
        // ...
    }





    bool RequestErrorHandling( WWW www )
    {
        // check the error code 401
        // ...

        // check for errors 
        if ( www.error != null )
        {
            Debug.Log( "WWW Error: " + www.error );

            return false;
        }

        return true;
    }

    public WWW GET( string url )
    {
        WWW www = new WWW(url); 

        // token!
        form.headers.Add( "Authorization", "Token " + token );

		return www;
    }

    public WWW POST( string url, Dictionary<string, string> postData )
    {
        WWWForm form = new WWWForm();

        postData.ForEach( each => form.AddFields( each.Key, each.Value ) );

        // token!
        form.headers.Add( "Authorization", "Token " + token );

        WWW www = new WWW( serverUrl + url, form );

        return www;
    }  
    */



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
