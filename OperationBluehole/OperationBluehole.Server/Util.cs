using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Server
{
    using OperationBluehole.Content;

    public static class Util
    {
        public static PlayerData ConvertToPlayerData( this Player player )
        {
            PlayerData playerData = new PlayerData();

            playerData.pId = player.pId;
            playerData.name = player.name;
            playerData.exp = player.exp;
            playerData.stats = new ushort[(int)StatType.StatCount];

            for ( int i = 0; i < (int)StatType.StatCount; ++i )
                playerData.stats[i] = player.baseStats[i];

            playerData.skills = new List<SkillId>();
            player.skills.ForEach( each => playerData.skills.Add( (SkillId)each ) );

            playerData.consumables = new List<ItemCode>();
            player.items.ForEach( each => playerData.consumables.Add( (ItemCode)each ) );

            playerData.equipments = new List<ItemCode>();
            player.equipments.ForEach( each => playerData.equipments.Add( (ItemCode)each ) );

            playerData.battleStyle = (BattleStyle)player.battleStyle;

            return playerData;
        }
    }
}
