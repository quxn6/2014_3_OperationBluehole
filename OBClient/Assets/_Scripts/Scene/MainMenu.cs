﻿using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public int DIFFICULT = GameConfig.DUNGEON_DIFFICULTY_DIFFICULT;
	public int NORMAL = GameConfig.DUNGEON_DIFFICULTY_NORMAL;
	public int EASY = GameConfig.DUNGEON_DIFFICULTY_EASY;

	public GameObject listObject = null;
	public GameObject floatingMenu = null;
	public GameObject replayButton = null;

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

	void Start()
	{
		// Load Player Data
		NetworkManager.Instance.GetPlayerInfo();

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
				Debug.Log( "Pooling Sequence" + i++ );
				NetworkManager.Instance.GetSimulationResult();
			}
			yield return new WaitForSeconds( GameConfig.RESULT_CHECKING_INTERVAL );
		}
	}

	public void PlaySimulationResult()
	{
		NetworkManager.Instance.IsRegisterd = false;
		Application.LoadLevel( "ReplayScene" );
	}

	public void FindParty(int difficulty)
	{
		NetworkManager.Instance.RegisterRequest( difficulty );
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
	}

	public void OnEnable()
	{
		SwipeMenu( MenuSwipeDirection.Default, 0.0f );
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

	public void ChangeItem()
	{

	}



}
