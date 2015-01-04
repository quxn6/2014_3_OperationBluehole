using UnityEngine;
using System.Collections;
using System.Text;

public class MainMenu : MonoBehaviour
{
	public int DIFFICULT = GameConfig.DUNGEON_DIFFICULTY_DIFFICULT;
	public int NORMAL = GameConfig.DUNGEON_DIFFICULTY_NORMAL;
	public int EASY = GameConfig.DUNGEON_DIFFICULTY_EASY;

	public GameObject listObject = null;
	public GameObject floatingMenu = null;
	public GameObject replayButton = null;
	public GameObject statusLabel;
	public GameObject playerCharacter;
	
	private UILabel statusText;

	// warning!!! for now, only 1 result can be checked
	//private bool hasResult = false;

	private enum MenuSwipeDirection
	{
		Left = 0 ,
		Default = 1,
		Right = 2 ,
		Down = 4 ,
		Up = 3,
	}

	void Awake()
	{
		statusText = statusLabel.GetComponent<UILabel>();
	}

	public void OnEnable()
	{
		SwipeMenu( MenuSwipeDirection.Default , 0.0f );
	}

	void Start()
	{
		// Register Actions
		NetworkManager.Instance.SetInventoryInfo = ItemManager.Instance.SetInventoryIcons;
		NetworkManager.Instance.SetStatusInfo = SetStatusWindow;

		// Load Player Data
		NetworkManager.Instance.LevelUpRequest();

		// Start Pooling Result
		StartCoroutine( PoolingResult() );
	}

	private IEnumerator PoolingResult()
	{
		int i = 0 ;
		// Check any simulation result wasn't played before
		NetworkManager.Instance.GetSimulationResult();

		while(true)
		{
			replayButton.gameObject.SetActive( ( NetworkManager.Instance.HasResult ) ? true : false );
			if ( !NetworkManager.Instance.HasResult && NetworkManager.Instance.IsRegisterd )
			{
				// Cannot request multiple at one time				
				Debug.Log( "Pooling Sequence" + i++ );
				NetworkManager.Instance.GetSimulationResult();
			}
			yield return new WaitForSeconds( GameConfig.RESULT_CHECKING_INTERVAL );
		}
	}

	public void FindParty( int difficulty )
	{
		if ( NetworkManager.Instance.IsRegisterd )
			return;

		NetworkManager.Instance.RegisterRequest( difficulty );
	}

	public void SetStatusWindow()
	{
		StringBuilder originalString = new StringBuilder();
		for ( int i = 0 ; i < DataManager.Instance.clientPlayerData.Stat.Count ; ++i )
		{
			originalString.Append( (OperationBluehole.Content.StatType)i );
			originalString.Append( " " );
			originalString.AppendLine( DataManager.Instance.clientPlayerData.Stat[i].ToString() );
		}
		originalString.Remove( originalString.Length - 1 , 1 );

		statusText.text = originalString.ToString();

		StatusManager.Instance.CalculateStatPoint();
	}

	public void PlaySimulationResult()
	{
		NetworkManager.Instance.HasResult = false;
		Application.LoadLevel( "ReplayScene" );
	}


	public void ChangeItem()
	{

	}

	public void ApplyStatUpResult()
	{
		StatusManager.Instance.ApplyStatUpResult();
	}

	private void SwipeMenu( MenuSwipeDirection swipeDirection, float swipeTime )
	{
		bool isVertical = false;
		if ( swipeDirection == MenuSwipeDirection.Up || swipeDirection == MenuSwipeDirection.Down)
		{
			isVertical = true;
		}

		float swipeAmountWidth = Screen.width / 2;
		float swipeAmountHeight = Screen.height;

		iTween.MoveTo( floatingMenu , iTween.Hash(
			"x" , ( isVertical ? (int)MenuSwipeDirection.Default : (int)swipeDirection ) * swipeAmountWidth ,
			"y" , ( isVertical ? ( (int)swipeDirection - (int)MenuSwipeDirection.Down ) : 0 ) * swipeAmountHeight ,
			"isLocal" , true ,
			"easetype" , iTween.EaseType.easeInOutExpo ,
			"time" , swipeTime
			) );

		iTween.MoveTo( playerCharacter , iTween.Hash(
			"x" , ( isVertical ? (int)MenuSwipeDirection.Default : (int)swipeDirection ) * 2.4f - 2.9f , // Got this value with NoGaDa
			"isLocal" , true ,
			"easetype" , iTween.EaseType.easeInOutExpo ,
			"time" , swipeTime
			) );
	}

	public void SelectDungeon()
	{
		SwipeMenu( MenuSwipeDirection.Right , GameConfig.MENU_SWIPE_TIME );
	}

	public void ReplayJournal()
	{		
		SwipeMenu( MenuSwipeDirection.Up , GameConfig.MENU_SWIPE_TIME );		
	}

	public void BackButton()
	{
		SwipeMenu( MenuSwipeDirection.Default , GameConfig.MENU_SWIPE_TIME );
	}

	public void MaintainCharacter()
	{
		SwipeMenu( MenuSwipeDirection.Left , GameConfig.MENU_SWIPE_TIME );
	}



}
