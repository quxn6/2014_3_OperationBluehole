using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour
{
	public GameObject loadingUI;
	public GameObject replayButton;
		
	void Start()
	{
		replayButton.SetActive( false );
		loadingUI.GetComponent<TweenAlpha>().enabled = false;
		LoadReplayInfo();
	}

	private void LoadReplayInfo()
	{
		// Init
		OperationBluehole.Content.ContentsPrepare.Init();
		OperationBluehole.Content.Player[] players = { new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() , new OperationBluehole.Content.Player() };

		// Warning!!! There is no information about mobPartyLevel in simulation result.
		// use temp variable
		int tempMobPartyLevel = 3;
		OperationBluehole.Content.Party playerParty = new OperationBluehole.Content.Party( OperationBluehole.Content.PartyType.PLAYER , tempMobPartyLevel );

		for ( int i = 0 ; i < DataManager.Instance.latestSimulationResult.PlayerList.Count ; ++i )
		{
			if ( DataManager.Instance.latestSimulationResult.PlayerList[i] == null )
			{
				Debug.LogError( "No Player " + i );
			}
			players[i].LoadPlayer( DataManager.Instance.latestSimulationResult.PlayerList[i] );
			playerParty.AddCharacter( players[i] );
		}

		LogGenerator.Instance.GenerateLog(
			DataManager.Instance.latestSimulationResult.MapSize ,
			DataManager.Instance.latestSimulationResult.Seed ,
			playerParty
			);
	}

	public void LoadMap(Dungeon dungeonMap)
	{
		MapManager.Instance.InitMapObjects();
		MapManager.Instance.PutMapObjects( dungeonMap );		
		replayButton.SetActive( true );
	}

	public void LoadReplayScene()
	{
		loadingUI.GetComponent<TweenAlpha>().enabled = true;
	}

	public void CloseLoadingView()
	{
		gameObject.SetActive( false );
	}
}
