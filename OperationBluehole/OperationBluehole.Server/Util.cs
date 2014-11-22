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
        public static PlayerData ConvertToPlayerData( this PlayerDataSource data )
        {
            PlayerData playerData = new PlayerData();

            playerData.pId = data.PlayerId;
            playerData.name = data.Name;
            playerData.exp = data.Exp;
            playerData.stats = new ushort[(int)StatType.StatCount];

            for ( int i = 0; i < (int)StatType.StatCount; ++i ) 
                playerData.stats[i] = data.Stat[i];

            playerData.skills = new List<SkillId>();
            data.Skill.ForEach( each => playerData.skills.Add( (SkillId)each ) );

            playerData.items = new List<ItemCode>();
            data.Consumable.ForEach( each => playerData.items.Add( (ItemCode)each ) );

            playerData.equipments = new List<ItemCode>();
            data.Equipment.ForEach( each => playerData.equipments.Add( (ItemCode)each ) );

            playerData.battleStyle = (BattleStyle)data.BattleStyle;

            return playerData;
        }

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

            playerData.items = new List<ItemCode>();
            player.items.ForEach( each => playerData.items.Add( (ItemCode)each ) );

            playerData.equipments = new List<ItemCode>();
            player.equipments.ForEach( each => playerData.equipments.Add( (ItemCode)each ) );

            playerData.battleStyle = (BattleStyle)player.battleStyle;

            return playerData;
        }
    }
}
