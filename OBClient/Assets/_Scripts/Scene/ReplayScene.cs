using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayScene : MonoBehaviour 
{
	// test gui
	void OnGUI()
	{
		if ( GUI.Button( new Rect( 10 , 10 , 100 , 50 ) , "Play Log" ) )
		{
			LogExecuter.Instance.PlayMapLog();
			//BattleManager.Instance.AssignBattleArea( 1 );
		}

		if ( GUI.Button( new Rect( 10 , 70 , 100 , 50 ) , "Quit Replay" ) )
		{
			LogExecuter.Instance.BackToMainMenu();
			//BattleManager.Instance.CleanBattleArea();
		}

// 		if ( GUI.Button( new Rect( 10 , 130 , 100 , 50 ) , "MoveCharacter" ) )
// 		{
// 			//LogExecuter.Instance.MoveCharacter( MapManager.Instance.PlayerParty, (MoveDirection)UnityEngine.Random.Range( 1 , 5 ) );
// 			
// 		}

// 		if ( GUI.Button( new Rect( 10 , 190 , 100 , 50 ) , "Player attack" ) )
// 		{
// 			LogExecuter.Instance.HeroAttackMob(0,1);
// 		}
// 
// 		if ( GUI.Button( new Rect( 10 , 250 , 100 , 50 ) , "Mob attack" ) )
// 		{
// 			StartCoroutine( LogExecuter.Instance.MobAttackHero(0,1) );
// 		}
	}
}
