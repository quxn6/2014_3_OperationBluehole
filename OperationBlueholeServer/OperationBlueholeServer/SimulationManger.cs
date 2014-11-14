using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using OperationBlueholeContent;

namespace OperationBlueholeServer
{
    public static class SimulationManger
    {
        static ConcurrentQueue<Party> waitingParties;
        static List<Thread> simulationThreads;
        static Random random;

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
                    DungeonMaster newMaster = new DungeonMaster();
                    newMaster.Init(60, random.Next(), party);
                    var res = newMaster.Start();
                    // TODO: 결과 처리
                }
                else
                    Thread.Yield();
            }
        }
    }
}