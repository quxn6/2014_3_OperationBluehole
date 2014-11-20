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
        public static PlayerData ConvertToPlayerData( this PlayerData_temp data )
        {
            PlayerData playerData = new PlayerData();

            playerData.name = data.Name;
            playerData.exp = data.Exp;
            playerData.stats = new ushort[(int)StatType.StatCount];

            for ( int i = 0; i < (int)StatType.StatCount; ++i ) 
                playerData.stats[i] = data.Stat[i];

            data.Skill.ForEach( each => playerData.skills.Add( (SkillId)each ) );
            data.Consumable.ForEach( each => playerData.items.Add( (ItemCode)each ) );
            data.Equipment.ForEach( each => playerData.equipments.Add( (ItemCode)each ) );

            playerData.battleStyle = (BattleStyle)data.BattleStyle;

            return playerData;
        }

        public static PlayerData ConvertToPlayerData( this Player player )
        {
            PlayerData playerData = new PlayerData();

            playerData.name = player.name;
            playerData.exp = player.exp;
            playerData.stats = new ushort[(int)StatType.StatCount];

            for ( int i = 0; i < (int)StatType.StatCount; ++i )
                playerData.stats[i] = player.baseStats[i];

            player.skills.ForEach( each => playerData.skills.Add( (SkillId)each ) );
            player.items.ForEach( each => playerData.items.Add( (ItemCode)each ) );
            player.equipments.ForEach( each => playerData.equipments.Add( (ItemCode)each ) );

            playerData.battleStyle = (BattleStyle)player.battleStyle;

            return playerData;
        }
    }
}
