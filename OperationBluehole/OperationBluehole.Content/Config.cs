using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
    static class Config
    {
        // Battle

        // Dungeon
        public const float MOB_DENSITY = 0.05f;
        public const float ITEM_DENSITY = 0.05f;
        public const int MAX_OFFSET = 2;
        public const float LEVEL_RANGE = 0.2f;

        public const int MININUM_SPAN = 12;
        public const int SLICE_WEIGHT_1 = 1;
        public const int SLICE_WEIGHT_2 = 2;
        public const int SILCE_WEIGHT_TOTAL = SLICE_WEIGHT_1 + SLICE_WEIGHT_2;

        // Character
		public const int MAX_CHARACTER_SP = 100;
		public static readonly ushort[] CHARACTER_BASE_STATS = { 1, 5, 5, 5, 5, 5, 5, 5 };
		public const short BATTLE_AI_RANDOM_LEVEL = 100;
		public const uint BATTLE_DEFENDED_MIN_DAMAGE = 1;

		// Mob
        public const int MOB_REWARD_EXP_WEIGHT = 10;
        public const int MOB_REWARD_GOLD_WEIGHT = 10;
        public const float MOB_REWARD_ITEM_RANGE = 0.2f;
		public const int MOB_REWARD_ITEM_CHANCE_MAX = 100000;

        // Party
        public const int MAX_PARTY_MEMBER = 4;
    }
}
