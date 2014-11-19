using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public GameObject listObject = null;
	public GameObject floatingMenu = null;

	private enum MenuSwipeDirection
	{
		Left = 0 ,
		Default = 1,
		Right = 2 ,
		Up = 3 ,
	}

	private void SwipeMenu( MenuSwipeDirection swipeDirection, float swipeTime )
	{
		if ( swipeDirection == MenuSwipeDirection.Up )
		{

			return;
		}

		float swipeAmount = Screen.width / 2;
		//floatingMenu.transform.localPosition = new Vector3( (int)swipeDirection * swipeAmount , 0.0f , 0.0f );

		iTween.MoveTo( floatingMenu , iTween.Hash(
			"x" , (int)swipeDirection * swipeAmount ,
			"isLocal", true,
			"easetype", iTween.EaseType.easeInOutExpo,
			"time", swipeTime
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
		//temp
		Application.LoadLevel( "ReplayScene" );
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
