using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LogType
{
	Move,
	Battle,
	Loot,
	Win,
	Fail,
}

public enum MoveDirection
{
	Stay = 0 ,
	Right = 1 ,
	Down = 2 ,
	Left = 3 ,
	Up = 4 ,
}

public struct LogInfo
{
	public LogType logType;
	public int logContent;

	public LogInfo ( LogType logType, int logContent)
	{
		this.logType = logType;
		this.logContent = logContent;
	}
}

//delegate void CharacterController(GameObject character);
//delegate void PlayerMoveQueue(GameObject character, MoveDirection direction);

public class LogExecuter : MonoBehaviour
{
// 	private GameObject playerParty = null;
// 	private List<GameObject> mobList = null;
	private Queue<LogInfo> replayLog;
	public Queue<LogInfo> ReplayLog
	{
		get { return replayLog; }
		set { replayLog = value; }
	}

	static private LogExecuter instance;
	static public LogExecuter Instance
	{
		get { return instance; }
	}

	private MoveDirection prevMoveDirection = MoveDirection.Stay;

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( this + " already exist" );
			return;
		}

		instance = this;
		replayLog = new Queue<LogInfo>();
	}

	// init variables
	public void InitLogExecuter()
	{
// 		playerParty = MapManager.Instance.PlayerParty;
// 		if ( playerParty == null )
// 			Debug.LogError( "Error (Log Executer) : There is No Player Party" );		
// 
// 		mobList = MapManager.Instance.MobList;
// 		if ( mobList == null )
// 			Debug.LogError( "Error (Log Executer) : There is No Mob List" );
	}

	public void PlayMapLog()
	{
		LogInfo logInfo = replayLog.Dequeue();		
		switch( logInfo.logType )
		{
			case LogType.Move :
				MoveCharacter( MapManager.Instance.PlayerParty , (MoveDirection)logInfo.logContent );
				break;
			case LogType.Battle :
				int dummyMobIndex = 1;
				PlayBattleLog(dummyMobIndex);

				// If we win the battle, deactivate mob
				MapManager.Instance.MobList[dummyMobIndex].SetActive( false );
				break;
			case LogType.Loot :
				int dummyItemIndex = 1;
				LootItem( dummyItemIndex );
				MapManager.Instance.ItemList[dummyItemIndex].SetActive( false );
				break;
			case LogType.Win:
				break;
			case LogType.Fail:
				break;
		}

		
	}

	private void Fail()
	{
		// show result popup
	}

	private void Clear()
	{
		// show result popup
	}

	private void LootItem(int index)
	{
		// show popup what is in the item box
		Debug.Log( index + "th item box opened" );
		
		// after few second or tap popup, Play log again
		PlayMapLog();
	}

	public void MoveCharacter( MoveDirection direction )
	{
		MoveCharacter( MapManager.Instance.PlayerParty , direction );
	}

	// Warning!!! Dirty Code. We should use coroutine here
	public void MoveCharacter( GameObject walker , MoveDirection direction )
	{
		IAnimatable animatable = (IAnimatable)walker.GetComponentInChildren( typeof( IAnimatable ) );
		animatable.PlayWalk();

		if ( prevMoveDirection == direction)
		{
			MoveOneBlock( walker );			
			return;
		}

		prevMoveDirection = direction;
		//character.transform.rotation = Quaternion.Euler( 0.0f , 90.0f * (float)direction , 0 );
		//character.transform.Translate( 0.0f , 0.0f , 1.0f , Space.Self );

		// Move After Rotation
		iTween.RotateTo( walker , iTween.Hash(
			"y" , GameConfig.CHARACTER_ROTATE_DEGREE * (float)direction ,
			"time" , GameConfig.CHARACTER_MOVING_TIME ,
			"islocal" , true ,
			"easetype" , iTween.EaseType.linear ,
			"oncomplete" , "MoveOneBlock" ,
			"oncompletetarget" , gameObject ,
			"oncompleteparams" , walker
			) );
	}

	private void MoveOneBlock( GameObject character )
	{
		iTween.MoveAdd( character , iTween.Hash(
				"z" , GameConfig.MINIMAL_UNIT_SIZE ,
				"time" , GameConfig.CHARACTER_MOVING_TIME ,
				"islocal" , true ,
				"easetype" , iTween.EaseType.linear ,
				"oncomplete" , "PlayMapLog" ,
				"oncompletetarget" , gameObject		
				) );
// 		iTween.MoveAdd( character , iTween.Hash(
// 				"z" , GameConfig.MINIMAL_UNIT_SIZE ,
// 				"time" , GameConfig.CHARACTER_MOVING_TIME ,
// 				"islocal" , true ,
// 				"easetype" , iTween.EaseType.linear ,
// 				"oncomplete" , "MoveCharacter" ,
// 				"oncompletetarget" , gameObject ,
// 				"oncompleteparams" , replayLog.Dequeue()
// 				) );
	}

	public void PlayBattleLog( int targetMobId )
	{
		Debug.Log( "Start battle!! with" + targetMobId );
		//BattleManager.Instance.StartBattle( targetMobId );
		PlayMapLog();
	}

	public void MobAttackHero( int mobNumber , int heroNumber , float damage )
	{
		StopAllCoroutines();
		StartCoroutine( MobAttackProcess(
			BattleManager.Instance.EnemyInstanceList[mobNumber] ,
			BattleManager.Instance.heroStatus[heroNumber] ,
			damage
			) );

		// heroes ui get damage( on UI )
	}

	IEnumerator MobAttackProcess( GameObject mob , GameObject hero , float damage )
	{
		IAnimatable animatableMob = ( (IAnimatable)mob.GetComponent( typeof( IAnimatable ) ) );

		// play attack animation
		animatableMob.PlayWalk();
		yield return new WaitForSeconds( GameConfig.MOB_ATTACKMOVING_TIME );
		animatableMob.PlayAttack();

		// after animation over, accept damage to hero
		while ( mob.GetComponent<Mob>().IsAnimationPlaying() )
		{
			yield return null;
		}

		hero.GetComponent<Hero>().BeAttacked( damage );
		animatableMob.PlayIdle();
	}

	public void HeroAttackMob( int mobNumber , int heroNumber , float damage )
	{
		BattleManager.Instance.EnemyInstanceList[mobNumber].GetComponent<Mob>().BeAttacked( damage );
	}

	public void KillPlayer( int heroesNumber )
	{
		BattleManager.Instance.heroStatus[heroesNumber].GetComponent<Hero>().BeKilled();
	}

	public void KillMob( int mobNumber )
	{
		( (IAnimatable)BattleManager.Instance.EnemyInstanceList[mobNumber].GetComponent( typeof( IAnimatable ) ) ).PlayDead();
	}

}
