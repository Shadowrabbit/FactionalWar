// ******************************************************************
//       /\ /|       @file       MapExtension.cs
//       \ V/        @brief      地图扩展
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-06 23:58:41
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public static class MapExtension
    {
        public static bool IsAllyExit(this Map map)
        {
            //没有派系
            //不是人类派系
            //当前角色是敌对派系
            //当前角色死亡 
            //当前角色倒地
            //当前角色崩溃
            //当前角色是殖民者
            //集群AI不存在
            //不是这个模组内的集群AI 不计在内
            //找到一个状态正常的友方单位 判定友军没有撤离地图
            return (from pawn in map.mapPawns.AllPawnsSpawned
                where pawn.Faction != null
                where pawn.Faction.def.humanlikeFaction
                where !pawn.Faction.HostileTo(Faction.OfPlayer)
                where !pawn.Dead
                where !pawn.Downed
                where !pawn.InMentalState
                where !pawn.IsColonist
                select pawn.GetLord()
                into lord
                where lord.LordJob != null
                select lord).All(lord =>
                !(lord.LordJob is LordJobFactionPairBase) && !(lord.LordJob is LordJobFactionContention));
        }
    }
}