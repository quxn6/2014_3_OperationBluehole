using UnityEngine;
using System.Collections;

public class LogGenerator : MonoBehaviour
{
	private GameObject sceneManager = null;
	private int battleLogIndex = 0;
	private int lootLogIndex = 0;

	static private LogGenerator instance;
	static public LogGenerator Instance
	{
		get { return instance; }
	}

	OperationBluehole.Content.DungeonMaster dungeonMaster;

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( this + " already exist" );
			return;
		}

		instance = this;

		// init private variables
		sceneManager = GameObject.FindGameObjectWithTag( "SceneManager" );
		dungeonMaster = new OperationBluehole.Content.DungeonMaster();
	}

	OperationBluehole.Content.DungeonMaster tmpMaster = new OperationBluehole.Content.DungeonMaster();
	public void GenerateLog( int size , int seed , OperationBluehole.Content.Party userParty )
	{
		// Generate Dungeon Map
		dungeonMaster.Init( size , seed , userParty );

		//////// warning!!!! ////////////////////
		// We should create deep copy method for MapObject. for now, just generate another dungeon with same seed.
		tmpMaster.Init( size , seed , userParty );
		/////////////////////////////////////////

		// Set object data
		DataManager.Instance.SetReplayMapData( userParty , dungeonMaster.mobs , dungeonMaster.items);

		// Get Dungeon Map(we generated) and Load on Client
		sceneManager.GetComponent<Loading>().LoadMap( new Dungeon( dungeonMaster.GetDungeonMap() , size ) );

		// Start Simulate and record results
		dungeonMaster.Start();

		// Write Log using record
		WriteLog();
	}

	// Sort Logtype using path infomation and write log queue 
	private void WriteLog()
	{
		// Set Looted Item Data
		DataManager.Instance.LootedItemList = dungeonMaster.record.lootedItems;

		// Set Battle Log for each battle
		LogExecuter.Instance.BattleLog = dungeonMaster.record.battleLog;

		// Set Replay Log for each turn
		char positionInfo = '\0';
		int xPos = 0;
		int yPos = 0;
		for ( int i = 0 ; i < dungeonMaster.record.pathfinding.Count - 1 ; ++i )
		{
			// check what is on the next step
			xPos = dungeonMaster.record.pathfinding[i + 1].x;
			yPos = dungeonMaster.record.pathfinding[i + 1].y;
			//int nextPosition = xPos * MapManager.Instance.InstanceDungeon.size + yPos;
			positionInfo = MapManager.Instance.InstanceDungeon.mapArray2D[yPos , xPos];

			// Generate Log and push in the queue
			LogExecuter.Instance.ReplayLog.Enqueue( MakeMoveLog( i ) );

			switch ( positionInfo )
			{
				case 'M': // start battle					
					LogExecuter.Instance.ReplayLog.Enqueue( MakeBattleLog( tmpMaster.GetMapObject( xPos , yPos ).party ) );
					break;
				case 'I': // loot item					
					LogExecuter.Instance.ReplayLog.Enqueue( MakeLootLog() );
					break;
				case 'T': // mob on the item, do two things
					LogExecuter.Instance.ReplayLog.Enqueue( MakeBattleLog( tmpMaster.GetMapObject( xPos , yPos ).party ) );
					LogExecuter.Instance.ReplayLog.Enqueue( MakeLootLog() );
					break;
				case 'O': // find exit
					LogExecuter.Instance.ReplayLog.Enqueue( MakeWinLog() );
					break;
				case ' ': // just move
					break;
				default:
					//Debug.LogError( "We can't go there" );
					break;
			}

			// Clear Used Map Data 
			MapManager.Instance.InstanceDungeon.mapArray2D[yPos , xPos] = ' ';
		}

		// Check dungeon had cleared
		if ( positionInfo != 'O' )
		{			
			LogExecuter.Instance.ReplayLog.Enqueue( MakeFailLog() );
		}

	}

	private LogInfo MakeFailLog()
	{
		return new LogInfo( LogType.Fail , 0 );
	}

	private LogInfo MakeWinLog()
	{
		return new LogInfo( LogType.Win , 0 );
	}

	private LogInfo MakeLootLog()
	{
		return new LogInfo( LogType.Loot , lootLogIndex++ );
	}

	// Set Encountered Mob Data
	private LogInfo MakeBattleLog( OperationBluehole.Content.Party mobParty )
	{
		DataManager.Instance.EncounteredMobPartyList.Add( mobParty );
		return new LogInfo( LogType.Battle , battleLogIndex++ );
	}

	private LogInfo MakeMoveLog( int index )
	{
		return new LogInfo(
				LogType.Move ,
				(int)ComputeDirection( dungeonMaster.record.pathfinding[index] , dungeonMaster.record.pathfinding[index + 1] )
				);
	}

	private MoveDirection ComputeDirection( OperationBluehole.Content.Int2D currentPos , OperationBluehole.Content.Int2D nextPos )
	{
		int xDiff = nextPos.x - currentPos.x;
		if ( xDiff != 0 )
		{
			if ( xDiff > 0 )
			{
				return MoveDirection.Right;
			}
			else
			{
				return MoveDirection.Left;
			}
		}

		int yDiff = nextPos.y - currentPos.y;
		if ( yDiff != 0 )
		{

			if ( yDiff > 0 )
			{
				return MoveDirection.Up;
			}
			else
			{
				return MoveDirection.Down;
			}
		}

		return MoveDirection.Stay;
	}
}
