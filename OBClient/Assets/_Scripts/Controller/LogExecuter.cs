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

public class LogExecuter : MonoBehaviour
{
	private MoveDirection prevMoveDirection = MoveDirection.Stay;

	public Queue<LogInfo> ReplayLog { get; private set; }
	//private List<List<Turn>>

	static private LogExecuter instance;
	static public LogExecuter Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( this + " already exist" );
			return;
		}

		instance = this;
		ReplayLog = new Queue<LogInfo>();
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
		LogInfo logInfo = ReplayLog.Dequeue();		
		switch( logInfo.logType )
		{
			case LogType.Move :
				MoveCharacter( MapManager.Instance.PlayerParty , (MoveDirection)logInfo.logContent );
				break;
			case LogType.Battle :
				PlayBattleLog();
				break;
			case LogType.Loot :
				LootItem();
				//MapManager.Instance.ItemList[dummyItemIndex].SetActive( false );
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

	private int lootedItemIterator = 0;
	private void LootItem()
	{
		// show popup what is in the item box
		var item = DataManager.Instance.LootedItemList[lootedItemIterator];
		Debug.Log( "Get Item " + item.code );

		// warning!!! : after user check popup, user tap popup and remove Itembox on map
		int itemIndex = item.position.y * MapManager.Instance.InstanceDungeon.size + item.position.x;
		GameObject itemInstance = null;
		if (!MapManager.Instance.ItemDictionary.TryGetValue( itemIndex , out itemInstance ))
		{
			Debug.LogError( "Error : Item list index is not exist" );
		}

		itemInstance.SetActive( false );
		++lootedItemIterator;
		
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
	}

	private int battleLogIterator = 0;
	public void PlayBattleLog()
	{
		Debug.Log( "Start battle!! with " + battleLogIterator + "th mob Party." );

		var mobParty = DataManager.Instance.EncounteredMobPartyList[battleLogIterator];
		if ( mobParty == null )
			Debug.LogError( "Error : There is no battle log" );

		//BattleManager.Instance.StartBattle()

		BattleManager.Instance.StartBattle( battleLogIterator );
		PlayMapLog();
	}

	public void EndBattle(OperationBluehole.Content.Party mobParty )
	{
		int mobIndex = mobParty.position.y * MapManager.Instance.InstanceDungeon.size + mobParty.position.x;
		GameObject mobInstance = null;
		if ( !MapManager.Instance.MobDictionary.TryGetValue( mobIndex , out mobInstance ) )
		{
			Debug.LogError( "Error : Mob list index is not exist" );
		}

		// If we win the battle, deactivate mob
		mobInstance.SetActive( false );
		++battleLogIterator;
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
