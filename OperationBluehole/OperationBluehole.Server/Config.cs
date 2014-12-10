using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationBluehole.Server
{
    public static class Config
    {
        public const int MATCHING_PARTY_MEMBERS_NUM = 4;
        public const int MATCHING_ALLOW_LEVEL_DIFF = 5;
        public const int MATCHING_STANDARD_POWER_PER_LEVEL = 5;
        public const int MATCHING_THREAD_END_DIFFCULTY = int.MaxValue;
        public const int MATCHING_PLAYER_THREAD_DEFAULT_NUM = 1;
        public const int MATCHING_PARTY_THREAD_DEFAULT_NUM = 1;

        public const int SIMULATION_THREAD_DEFAULT_NUM = 4;
    }
}