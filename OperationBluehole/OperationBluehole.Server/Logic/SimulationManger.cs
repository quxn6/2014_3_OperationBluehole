using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using OperationBluehole.Content;

namespace OperationBluehole.Server
{
    public static class SimulationManger
    {
        static ConcurrentQueue<Party> waitingParties;
        static List<Thread> simulationThreads;
        static Random random;

        static long resultIdx = 0;

        public static void Init()
        {
            waitingParties = new ConcurrentQueue<Party>();
            simulationThreads = new List<Thread>();
            random = new Random((int)Stopwatch.GetTimestamp());

            // 전투 로직 초기화
            ContentsPrepare.Init();

            for (int i = 0; i < Config.MATCHING_PLAYER_THREAD_DEFAULT_NUM; ++i)
            {
                Thread newThread = new Thread(SimulationParty);
                newThread.Start();
                simulationThreads.Add(newThread);
            }

            Console.WriteLine("simulation manager is started");
        }

        public static void RegisterParty(Party party)
        {
            waitingParties.Enqueue(party);
        }

        static void SimulationParty()
        {
            Party party;
            while (true)
            {
                if (waitingParties.TryDequeue(out party))
                {
                    Console.WriteLine( "start to simulation" );

                    SimulationResult result = new SimulationResult();
                    
                    long currentIdx = Interlocked.Increment( ref resultIdx );
                    // over flow 확인하고 제어 할 것
                    
                    result.Id = currentIdx;
                    result.Seed = random.Next();

                    // result에 사용될 사용자 정보 기록
                    result.PlayerList = new List<PlayerData>();
                    party.characters.ForEach( each =>
                    {
                        var player = (Player)each;
                        result.PlayerList.Add( player.ConvertToPlayerData() );
                    } );

                    // simulation!!!!
                    DungeonMaster newMaster = new DungeonMaster();
                    newMaster.Init( 60, result.Seed, party );
                    var res = newMaster.Start();

                    // save the result data in DB
                    result.CheckedPlayer = new List<ulong>();
                    Debug.Assert( SimulationResultDatabase.SetSimulationResult( result ) );

                    party.characters.ForEach( each =>
                    {
                        string id = ( (Player) each ).pId;

                        // update player data
                        var playerData = PlayerDataDatabase.GetPlayerData( id );
                        var userData = UserDataDatabase.GetUserData( id );
                        
                        // 크게 할 일은 경험치 추가, 골드 추가, 아이템토큰 추가
                        userData.Gold += newMaster.record.lootedGold;
                        playerData.exp += newMaster.record.lootedExp;
                        newMaster.record.lootedItems.ForEach( eachItem => { userData.Token.Add( (ItemToken)eachItem ); } );

                        Debug.Assert( UserDataDatabase.SetUserData( userData ) );
                        Debug.Assert( PlayerDataDatabase.SetPlayerData( playerData ) );

                        // update result table
                        var resultTable = ResultTableDatabase.GetResultTable( id );
                        Debug.Assert( resultTable.UnreadId == -1, "unread result remains" );

                        resultTable.UnreadId = currentIdx;
                        Debug.Assert( ResultTableDatabase.SetResultTable( resultTable ), "fail to save the result table - ID : " + resultTable.PlayerId );
                    } );

                    Console.WriteLine( "simulation ended" );
                }
                else
                    Thread.Yield();
            }
        }
    }
}