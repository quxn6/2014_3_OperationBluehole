using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Lobby
{
	using System.Diagnostics;
	using RabbitMQ.Client;
	using OperationBluehole.Content;
	using OperationBluehole.Database;

	class WorkerData
	{
		public ConnectionFactory factory;
		public long queueNum;
		public ushort minLev;
		public ushort maxLev;
	}

	class MatchingManager
	{
		SynchronizedCollection<WorkerData> workerList;

		public MatchingManager()
		{
			workerList = new SynchronizedCollection<WorkerData>();
		}

		public bool RegisterPlayer( string playerId, int difficulty )
		{
			var playerData = PlayerDataDatabase.GetPlayerData( playerId );
			Debug.Assert( playerData != null, "player data is null : " + playerId );

			int lev = playerData.stats[(int)StatType.Lev];
			if ( workerList.Count - 1 < lev || workerList[lev] == null )
				return false;

			var newDic = new Dictionary<string, object>();
			newDic.Add( "playerId", playerId );
			newDic.Add( "difficulty", difficulty );
			TaskManager.SendToWorker( workerList[lev], "REG", newDic );
			return true;
		}

		public bool DeregisterPlayer( string playerId )
		{
			var playerData = PlayerDataDatabase.GetPlayerData( playerId );
			Debug.Assert( playerData != null, "player data is null : " + playerId );

			int lev = playerData.stats[(int)StatType.Lev];
			if ( workerList.Count - 1 < lev || workerList[lev] == null )
				return false;

			var newDic = new Dictionary<string, object>();
			newDic.Add( "playerId", playerId );
			TaskManager.SendToWorker( workerList[lev], "DEREG", newDic );
			return true;
		}

		public void SetWorker( string host, int queueNum, ushort minLev, ushort maxLev )
		{
			var cf = new ConnectionFactory() { HostName = host };
			var wData = new WorkerData();
			wData.factory = cf;
			wData.queueNum = queueNum;
			wData.minLev = minLev;
			wData.maxLev = maxLev;

			int listMaxNum = workerList.Count - 1;

			if ( listMaxNum < wData.minLev - 1 )
			{
				for ( int i = 0; i < wData.minLev - 1 - listMaxNum; ++i )
				{
					workerList.Add( null );
				}
				for ( int i = wData.minLev; i <= wData.maxLev; ++i )
				{
					workerList.Add( wData );
				}
			}
			else if ( listMaxNum < wData.maxLev )
			{
				SortedSet<WorkerData> removed = new SortedSet<WorkerData>(); // 중복 Add안하려고 Set을 쓰려는데 Sorted밖에 없네...
				for ( int i = wData.minLev; i <= listMaxNum; ++i )
				{
					if(workerList[i] != null)
						removed.Add( workerList[i] );
					workerList[i] = wData;
				}
				for ( int i = listMaxNum + 1; i <= wData.maxLev; ++i )
				{
					workerList.Add( wData );
				}

				foreach ( var item in removed )
				{
					if ( item.maxLev >= wData.minLev )
						item.maxLev = (ushort)(wData.minLev - 1);
					else
						item.maxLev = 0;

					var newDic = new Dictionary<string, object>();
					newDic.Add( "minLev", item.minLev );
					newDic.Add( "maxLev", item.maxLev );
					TaskManager.SendToWorker( item, "SET", newDic );
				}
			}
			else
			{
				SortedSet<WorkerData> removed = new SortedSet<WorkerData>(); // 중복 Add안하려고 Set을 쓰려는데 Sorted밖에 없네...
				for ( int i = minLev; i <= wData.maxLev; ++i )
				{
					if ( workerList[i] != null )
						removed.Add( workerList[i] );
					workerList[i] = wData;
				}

				foreach ( var item in removed )
				{
					// 해당 구간과 겹치는 Worker 조정
					//       [Worker     ]
					// [new Worker]
					if ( item.minLev <= wData.minLev && wData.maxLev < item.maxLev )
						item.minLev = (ushort)(wData.maxLev + 1);

					// 해당 구간과 겹치는 Worker 조정
					// [Worker     ]
					//        [new Worker]
					else if ( item.minLev < wData.minLev && wData.minLev <= item.maxLev )
						item.maxLev = (ushort)(wData.minLev - 1);

					// 해당 구간과 겹치는 Worker 조정
					// [Worker            ]
					//     [new Worker]
					else if ( item.minLev < wData.minLev && wData.maxLev < item.maxLev )
					{
						// 조심해!! 사실 이 경우 본래 있던 Worker가 2개로 분할되어야 하지만 
						//			일단 작은 구간을 추가된 Worker에 붙이는 식으로 처리.

						if ( minLev - item.minLev > item.maxLev - wData.maxLev )
						{
							wData.maxLev = item.maxLev;
							item.maxLev = (ushort)(wData.minLev - 1);
						}
						else
						{
							wData.minLev = item.minLev;
							item.minLev = (ushort)(wData.maxLev + 1);
						}

						// 추가된 Worker 재설정
						var tDic = new Dictionary<string, object>();
						tDic.Add( "minLev", wData.minLev );
						tDic.Add( "maxLev", wData.maxLev );
						TaskManager.SendToWorker( wData, "SET", tDic );
					}

					// 해당 구간에 포함되는 Worker 삭제 설정
					//      [Worker]
					// [new Worker        ]
					else
						item.maxLev = 0;


					// 겹친 Worker 재설정
					var newDic = new Dictionary<string, object>();
					newDic.Add( "minLev", item.minLev );
					newDic.Add( "maxLev", item.maxLev );
					TaskManager.SendToWorker( item, "SET", newDic );
				}
			}

			Console.WriteLine( "Set Worker {0} ~ {1}", wData.minLev, wData.maxLev );
		}
	}
}
