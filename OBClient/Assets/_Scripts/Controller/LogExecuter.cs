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
	public List<List<OperationBluehole.Content.TurnInfo>> BattleLog {get; set;}

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

	// init battle log for each battle
	public void InitLogExecuter(List<List<OperationBluehole.Content.TurnInfo>> battleLog)
	{
		this.BattleLog = battleLog;
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
				StartCoroutine( PlayBattleLog() );
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
// 	public void PlayBattleLog()
// 	{
// 		Debug.Log( "Start battle!! with " + battleLogIterator + "th mob Party." );
// 
// 		var mobParty = DataManager.Instance.EncounteredMobPartyList[battleLogIterator];
// 		if ( mobParty == null )
// 			Debug.LogError( "Error : There is no battle log" );
// 
// 		BattleManager.Instance.AssignBattleArea( battleLogIterator );
// 		//PlayBattleLog();
// 	}

	IEnumerator PlayBattleLog()
	{
		Debug.Log( "Start battle!! with " + battleLogIterator + "th mob Party." );

		var mobParty = DataManager.Instance.EncounteredMobPartyList[battleLogIterator];
		if ( mobParty == null )
		{
			Debug.LogError( "Error : There is no battle log" );
		}			

		BattleManager.Instance.AssignBattleArea( battleLogIterator );

		yield return null;
		
		// Process battle continue
		for ( int i = 0 ; i < BattleLog[battleLogIterator].Count; ++i)
		{
			yield return StartCoroutine( EachTurn( BattleLog[battleLogIterator][i] ) );
		}

		EndBattle(mobParty);
	}
	
	IEnumerator EachTurn( OperationBluehole.Content.TurnInfo turnInfo )
	{
		// Do source job
		GameObject source = (turnInfo.srcType == OperationBluehole.Content.PartyType.MOB)?
			BattleManager.Instance.EnemyInstanceList[turnInfo.srcIdx] :
			BattleManager.Instance.heroStatus[turnInfo.srcIdx];
		
		// Warning!!! We could be make base class(like character or something) for making Mob and Hero derived class
		// Warning!!! For now, we only use just attack skill, which skill used 
		switch( turnInfo.srcType)
		{
			case OperationBluehole.Content.PartyType.MOB :
				yield return StartCoroutine( source.GetComponent<Mob>().Attack() );
				break;			
			case OperationBluehole.Content.PartyType.PLAYER:
				yield return StartCoroutine( source.GetComponent<Hero>().Attack() );
				break;
		}

		// Do target job
		ApplyDamage( turnInfo.targets );
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
		BattleManager.Instance.CleanBattleArea();
		PlayMapLog();
	}

	// Iterate target's data and apply damage
	// Damage in TargetAffected struct is positive value, so when we update target data, multiply (-) at its value.
	private void ApplyDamage(List<OperationBluehole.Content.TargetAffected> targetDataList)
	{		
		for ( int i = 0 ; i < targetDataList.Count; ++i)
		{
			Debug.Log( targetDataList[i].value );
			// apply damage data & play animation for hit
			switch ( targetDataList[i].targetType )
			{
				case OperationBluehole.Content.PartyType.MOB:					
					Mob mob = BattleManager.Instance.EnemyInstanceList[targetDataList[i].targetIdx].GetComponent<Mob>();					
					mob.UpdateCharacterData( targetDataList[i].gaugeType , targetDataList[i].value );					
					mob.BeAttacked();
					break;
				case OperationBluehole.Content.PartyType.PLAYER:
					Hero hero = BattleManager.Instance.heroStatus[i].GetComponent<Hero>();
					hero.UpdateCharacterData( targetDataList[i].gaugeType , targetDataList[i].value );
					hero.BeAttacked();
					break;
			}
		}
	}
		
	//public IEnumerator MobAttackHero( int mobNumber , int heroNumber , float damage )
	public IEnumerator MobAttackHero( GameObject mob, GameObject hero )
	{
		// Do Attack Process
		yield return StartCoroutine( mob.GetComponent<Mob>().Attack() );
		
		// heroes ui get damage( on UI )
		//hero.GetComponent<Hero>().BeAttacked( damage );

		//PlayBattleLog();
	}

// 	public void HeroAttackMob( GameObject mob , GameObject hero )
// 	{
// 		mob.GetComponent<Mob>().BeAttacked( damage );
// 	}

	public void KillPlayer( GameObject hero)
	{
		hero.GetComponent<Hero>().BeKilled();
	}

	public void KillMob( GameObject mob )
	{
		( (IAnimatable)mob.GetComponent( typeof( IAnimatable ) ) ).PlayDead();
	}

}
