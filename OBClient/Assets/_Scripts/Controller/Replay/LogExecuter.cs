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
				Clear();
				break;
			case LogType.Fail:
				Fail();
				break;
		}
	}

	private void Fail()
	{
		Debug.Log( "Meet Fail" );
		// show result popup
		BackToMainMenu();
	}

	private void Clear()
	{
		Debug.Log( "Meet Win" );
		// show result popup
	}

	public void BackToMainMenu()
	{
		MapManager.Instance.ClearAllMapObjects();
		Application.LoadLevel( "MainMenu" );
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
			//Debug.LogError( "Error : Item list index is not exist" );
			// temporary
			Debug.Log( "Clean Failed (" + item.position.x + " , " + item.position.y + ")" );
			++lootedItemIterator;
			PlayMapLog();
			return;
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
	IEnumerator PlayBattleLog()
	{
		Debug.Log( "Start battle!! with " + battleLogIterator + "th mob Party." );

		// Set Mob Party
		var mobParty = DataManager.Instance.EncounteredMobPartyList[battleLogIterator];
		if ( mobParty == null )
		{
			Debug.LogError( "Error : There is no battle log" );
		}

		// Set Battle Area
		BattleManager.Instance.AssignBattleArea( battleLogIterator );

		yield return null;
		
		// Execute Each turn action
		for ( int i = 0 ; i < BattleLog[battleLogIterator].Count; ++i)
		{
			yield return StartCoroutine( EachTurn( BattleLog[battleLogIterator][i] ) );
		}

		EndBattle(mobParty);
	}
	
	IEnumerator EachTurn( OperationBluehole.Content.TurnInfo turnInfo )
	{
		if ( turnInfo.skillId == OperationBluehole.Content.SkillId.None )
			yield break;

		// Do source job
		GameObject source = (turnInfo.srcType == OperationBluehole.Content.PartyType.MOB)?
			BattleManager.Instance.EnemyInstanceList[turnInfo.srcIdx] :
			BattleManager.Instance.heroStatus[turnInfo.srcIdx];
		
		// Warning!!! We could be make base class(like character or something) for making Mob and Hero derived class
		// Warning!!! For now, we only use just attack skill, which skill used 
		switch( turnInfo.srcType)
		{
			case OperationBluehole.Content.PartyType.MOB:
				Debug.Log( "turn start" );
				yield return StartCoroutine( source.GetComponent<Mob>().Attack(turnInfo) );		
				Debug.Log( "turn end" );		
				break;			
			case OperationBluehole.Content.PartyType.PLAYER:
				yield return StartCoroutine( source.GetComponent<Hero>().Attack() );
				break;
		}

		// Do target job
		ApplyDamage( turnInfo.targets );
	}

	private int counter = 0;
	public void EndBattle(OperationBluehole.Content.Party mobParty )
	{
		++counter;
		Debug.Log( counter );
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
			//Debug.Log( targetDataList[i].value );
			// apply damage data & play animation for hit
			switch ( targetDataList[i].targetType )
			{
				case OperationBluehole.Content.PartyType.MOB:					
					Mob mob = BattleManager.Instance.EnemyInstanceList[targetDataList[i].targetIdx].GetComponent<Mob>();					
					mob.UpdateCharacterData( targetDataList[i].gaugeType , targetDataList[i].value );					
					mob.BeAttacked();
					break;
				case OperationBluehole.Content.PartyType.PLAYER:
					Hero hero = BattleManager.Instance.heroStatus[targetDataList[i].targetIdx].GetComponent<Hero>();
					hero.UpdateCharacterData( targetDataList[i].gaugeType , targetDataList[i].value );
					hero.BeAttacked();
					break;
			}
		}
	}
}
