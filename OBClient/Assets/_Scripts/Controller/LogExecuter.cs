using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MoveDirection
{
	STAY = 0,
	LEFT = 1,
	DOWN = 2,
	RIGHT = 3,
	UP = 4,
}

//delegate void CharacterController(GameObject character);

public class LogExecuter : MonoBehaviour
{
	private GameObject playerParty = null;
	private List<GameObject> mobList = null;

	// init variables
	public void InitLogExecuter()
	{
		playerParty = MapManager.Instance.PlayerParty;
		if ( playerParty == null )
			Debug.LogError( "Error (Log Executer) : There is No Player Party" );

		mobList = MapManager.Instance.MobList;
		if ( mobList == null)
			Debug.LogError( "Error (Log Executer) : There is No Mob List" );
	}

	// Warning!!! Dirty Code. We should use coroutine here
	public void MoveCharacter( GameObject character , MoveDirection direction )
	{
		IAnimatable animatable = (IAnimatable)character.GetComponent( typeof( IAnimatable ) );
		animatable.PlayWalk();

		//character.transform.rotation = Quaternion.Euler( 0.0f , 90.0f * (float)direction , 0 );
		//character.transform.Translate( 0.0f , 0.0f , 1.0f , Space.Self );
		
		// Move After Rotation
		iTween.RotateTo( character , iTween.Hash(
			"y" , GameConfig.CHARACTER_ROTATE_DEGREE * (float)direction ,
			"time" , GameConfig.CHARACTER_MOVING_TIME ,
			"islocal" , true ,
			"easetype" , iTween.EaseType.linear ,
			"oncomplete" , "MoveOneBlock",
			"oncompletetarget" , gameObject ,
			"oncompleteparams" , character
			) );
	}

	private void MoveOneBlock( GameObject character )
	{
		iTween.MoveAdd( character, iTween.Hash(
				"z" , GameConfig.MINIMAL_UNIT_SIZE ,
				"time" , GameConfig.CHARACTER_MOVING_TIME ,
				"islocal" , true ,
				"easetype" , iTween.EaseType.linear
				) );
	}

	public void StartBattle(GameObject target)
	{
		Debug.Log( "Start battle!! with" + target );
	}

	// test gui
	void OnGUI()
	{
		if ( GUI.Button( new Rect( 10 , 10 , 100 , 50 ) , "Init LogExe" ) )
		{
			InitLogExecuter();
		}

		if ( GUI.Button( new Rect( 10 , 70 , 100 , 50 ) , "MoveCharacter" ) )
		{
			MoveCharacter( playerParty , (MoveDirection)UnityEngine.Random.Range( 1 , 5 ) );
		}

		if ( GUI.Button( new Rect( 10 , 130 , 100 , 50 ) , "Init Battle Part" ) )
		{			
			BattleManager.Instance.InitBattleObjects();
			BattleManager.Instance.PutBattleObjects( 1 );
		}
	}
}
