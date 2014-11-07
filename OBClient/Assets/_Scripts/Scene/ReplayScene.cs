using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayScene : MonoBehaviour 
{
	

	// test gui
	void OnGUI()
	{
		if ( GUI.Button( new Rect( 10 , 10 , 100 , 50 ) , "Init LogExe" ) )
		{
			LogExecuter.Instance.InitLogExecuter();
			DataManager.Instance.InitEnemyDataList();
			DataManager.Instance.InitUserDataList();
		}

		if ( GUI.Button( new Rect( 10 , 70 , 100 , 50 ) , "MoveCharacter" ) )
		{
			LogExecuter.Instance.MoveCharacter( MapManager.Instance.PlayerParty, (MoveDirection)UnityEngine.Random.Range( 1 , 5 ) );
		}

		if ( GUI.Button( new Rect( 10 , 130 , 100 , 50 ) , "Init Battle Part" ) )
		{
			BattleManager.Instance.StartBattle( 1 );
		}
	}
}
