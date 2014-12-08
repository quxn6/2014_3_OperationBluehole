﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using OperationBluehole.Content;

namespace OperationBluehole.Server
{
    struct MatchingData
    {
        public Object lockObj;
        public Int64 regTime;
        public ushort level;
        public int difficulty;
        public List<Tuple<Player, int, List<string>>> members;
    }

    public static class MatchingManager
    {
        static ConcurrentQueue<Tuple<Player,int,List<string>>> waitingPlayers; // Tuple<플레이어, 추가난이도, 차단리스트, 종료코드>
        
        static SynchronizedCollection<string> deregisterWaitingPlayers;
        static SynchronizedCollection<MatchingData> waitingParties;
        
        static ConcurrentQueue<MatchingData> matchedParties;

        static List<Thread> matchPlayerThreads;
        static List<Thread> matchPartyThreads;
        
        public static void RegisterPlayer( string playerId, int difficulty )
        {
            Player newPlayer = new Player();

            var playerData = PlayerDataDatabase.GetPlayerData( playerId );
            Debug.Assert( playerData != null, "player data is null : " + playerId );

            // 이미 등록되어 있는지 확인 필요
            // ...

            var userData = UserDataDatabase.GetUserData( playerId );
            Debug.Assert( userData != null, "user data is null : " + playerId );

            newPlayer.LoadPlayer( playerData );

            Console.WriteLine( "player id : " + newPlayer.pId );

            waitingPlayers.Enqueue( new Tuple<Player, int, List<string>>( newPlayer, difficulty, userData.BanList ) );
        }

        public static void DeregisterPlayer(string playerId)
        {
            deregisterWaitingPlayers.Add(playerId);

            int idx = -1;
            MatchingData res = waitingParties.FirstOrDefault(md => {
                idx = md.members.FindIndex(p => p.Item1.pId == playerId);
                return idx > -1;
            });

            if (idx > -1)
            {
                deregisterWaitingPlayers.Remove(playerId);
                res.members.RemoveAt(idx);
                return;
            }
        }

        public static void Init()
        {
            waitingPlayers = new ConcurrentQueue<Tuple<Player, int, List<string>>>();
            
            deregisterWaitingPlayers = new SynchronizedCollection<string>();
            waitingParties = new SynchronizedCollection<MatchingData>();

            matchedParties = new ConcurrentQueue<MatchingData>();

            matchPlayerThreads = new List<Thread>();
            matchPartyThreads = new List<Thread>();

            for (int i = 0; i < Config.MATCHING_PARTY_THREAD_DEFAULT_NUM; ++i)
            {
                Thread newThread = new Thread(MatchParty);
                newThread.Start();
                matchPartyThreads.Add(newThread);
            }
            for (int i = 0; i < Config.MATCHING_PLAYER_THREAD_DEFAULT_NUM; ++i)
            {
                Thread newThread = new Thread(MatchPlayer);
                newThread.Start();
                matchPlayerThreads.Add(newThread);
            }

            Console.WriteLine( "matching manager is started" );
        }

        static void MatchPlayer()
        {
            Tuple<Player, int, List<string>> player;
            while (true)
            {
                if (waitingPlayers.TryDequeue(out player))
                {
                    if (player.Item2 == Config.MATCHING_THREAD_END_DIFFCULTY) // 종료
                        return;

                    if (deregisterWaitingPlayers.Contains(player.Item1.pId)) // 등록 플레이어 제거
                    {
                        deregisterWaitingPlayers.Remove(player.Item1.pId);
                        return;
                    }

                    ParamType maxParam = ParamType.phyAtk;
                    for (int i = 1; i < 4; ++i)
                    {
                        if (player.Item1.actualParams[(int)maxParam] < player.Item1.actualParams[i])
                            maxParam = (ParamType)i;
                    }

                    var resList = waitingParties.AsParallel()
                        .Where(md => Math.Abs(md.level - player.Item1.baseStats[(int)StatType.Lev]) <= Config.MATCHING_ALLOW_LEVEL_DIFF)
                        .Where(md => md.difficulty == player.Item2)
                        .Where(md => !md.members.Exists(p => p.Item3.Contains(player.Item1.pId)))
                        .OrderBy(md => md.regTime);

                    if (resList.Count() == 0)
                    {
                        MatchingData newMd = new MatchingData();
                        newMd.lockObj = new Object();
                        newMd.members = new List<Tuple<Player, int, List<string>>>();
                        newMd.members.Add(player);
                        newMd.level = player.Item1.baseStats[(int)StatType.Lev];
                        newMd.difficulty = player.Item2;
                        newMd.regTime = Stopwatch.GetTimestamp();

                        waitingParties.Add(newMd);
                        continue;
                    }

                    foreach (var md in resList)
                    {
                        lock (md.lockObj)
                        {
                            if (md.members.Count < Config.MATCHING_PARTY_MEMBERS_NUM)
                                md.members.Add(player);
                        }
                        if (md.members.Count == Config.MATCHING_PARTY_MEMBERS_NUM)
                        {
                            matchedParties.Enqueue(md);
                            waitingParties.Remove(md);
                            break;
                        }
                    }
                }
                else
                    Thread.Yield();
            }
        }

        static void MatchParty()
        {
            MatchingData md;
            while (true)
            {
                if (matchedParties.TryDequeue(out md))
                {
                    int power = 0;
                    for (int i = 0; i < 4; ++i)
                        power += (int)md.members.Average(c => c.Item1.actualParams[i]);

                    int partyLevel = power / Config.MATCHING_STANDARD_POWER_PER_LEVEL;
                    partyLevel += md.difficulty;
                    if (partyLevel < 1)
                        partyLevel = 1;

                    Party newParty = new Party(PartyType.PLAYER, partyLevel);
                    foreach (var p in md.members)
                    {
                        newParty.AddCharacter(p.Item1);
                    }

                    SimulationManger.RegisterParty(newParty);
                }
                else
                    Thread.Yield();
            }
        }
    }
}