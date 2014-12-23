using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Simulation
{
    using OperationBluehole.Content;
    using OperationBluehole.Database;
    using System.Diagnostics;
    using System.Threading;

    public static class SimulationManager
    {
        // static ConcurrentQueue<Party> waitingParties;
        // static List<Thread> simulationThreads;
        static Random random;

        static long resultIdx = 0;

        public static void Init()
        {
            // waitingParties = new ConcurrentQueue<Party>();
            // simulationThreads = new List<Thread>();
            random = new Random( (int)Stopwatch.GetTimestamp() );

            // 전투 로직 초기화
            ContentsPrepare.Init();

            Debug.WriteLine( "simulation manager is started" );
        }

        public static void Simulation( Party party )
        {
            Debug.WriteLine( "start to simulation" );
            Console.WriteLine( "start to simulation" );

            SimulationResult result = new SimulationResult();

            long currentIdx = Interlocked.Increment( ref resultIdx );
            // over flow 확인하고 제어 할 것

            result.Id = currentIdx;
            result.Seed = random.Next();
            result.MapSize = 60;

            // result에 사용될 사용자 정보 기록
            result.PlayerList = new List<PlayerData>();
            party.characters.ForEach( each =>
            {
                var player = (Player)each;
                result.PlayerList.Add( player.ConvertToPlayerData() );
            } );

            Console.WriteLine( "party level : " + party.partyLevel );

            // simulation!!!!
            DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( result.MapSize, result.Seed, party );
            var res = newMaster.Start();

            Console.WriteLine( "turn : " + res );
            Console.WriteLine( "looted gold : " + newMaster.record.lootedGold );
            Console.WriteLine( "looted exp : " + newMaster.record.lootedExp );

            // save the result data in DB
            result.CheckedPlayer = new List<ulong>();
            Debug.Assert( SimulationResultDatabase.SetSimulationResult( result ) );

            party.characters.ForEach( each =>
            {
                string id = ( (Player)each ).pId;

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

                // update rank
                Debug.Assert( RedisManager.UpdateRank( playerData.pId, playerData.GetScore() ) );
            } );

            Debug.WriteLine( "simulation ended" );
            Console.WriteLine( "simulation ended" );
        }
    }
}
