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

	public void GenerateLog( int size , int seed , OperationBluehole.Content.Party userParty )
	{
		// Generate Dungeon Map
		dungeonMaster.Init( size , seed , userParty );

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
		char positionInfo = '\0';
		for ( int i = 0 ; i < dungeonMaster.record.pathfinding.Count - 1 ; ++i )
		{
			// check what is on the next step
			int nextPosition = 
				dungeonMaster.record.pathfinding[i+1].y * MapManager.Instance.InstanceDungeon.size 
				+ dungeonMaster.record.pathfinding[i+1].x;
			positionInfo = MapManager.Instance.InstanceDungeon.mapArray[nextPosition] ;

			// Generate Log and push in the queue
			LogExecuter.Instance.ReplayLog.Enqueue( MakeMoveLog( i ) );

			switch ( positionInfo )
			{
				case 'M': // start battle					
					LogExecuter.Instance.ReplayLog.Enqueue( MakeBattleLog() );
					break;
				case 'I': // loot item					
					LogExecuter.Instance.ReplayLog.Enqueue( MakeLootLog() );
					break;
				case 'T': // mob on the item, do two things
					LogExecuter.Instance.ReplayLog.Enqueue( MakeBattleLog() );
					LogExecuter.Instance.ReplayLog.Enqueue( MakeLootLog() );
					break;
				case 'O': // find exit
					LogExecuter.Instance.ReplayLog.Enqueue( MakeWinLog() );
					break;
			}

			// Clear Used Map Data 
			MapManager.Instance.InstanceDungeon.mapArray[nextPosition] = ' ';
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

	private LogInfo MakeBattleLog()
	{
		return new LogInfo( LogType.Battle , battleLogIndex++ );
	}

	private LogInfo MakeMoveLog(int index)
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
