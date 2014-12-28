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
		Down = 4 ,
		Up = 3,
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
