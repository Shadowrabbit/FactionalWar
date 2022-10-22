// ******************************************************************
//       /\ /|       @file       FactionUtil.cs
//       \ V/        @brief      事件工具
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 13:21:13
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public static class FactionUtil
    {
        /// <summary>
        /// 获取两个相互敌对的派系
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pawnGroupKindDef"></param>
        /// <param name="candidateFactionList"></param>
        /// <param name="faction1"></param>
        /// <param name="faction2"></param>
        /// <param name="validator"></param>
        public static void GetHostileFactionPair(out Faction faction1,
            out Faction faction2, float points, PawnGroupKindDef pawnGroupKindDef,
            List<Faction> candidateFactionList, Predicate<Faction> validator = null)
        {
            faction1 = null;
            faction2 = null;
            //乱序
            candidateFactionList.Shuffle();
            //需要存在两个互相敌对的派系
            foreach (var faction in candidateFactionList)
            {
                //无效派系
                if (!faction.IsFactionEffective(points, pawnGroupKindDef))
                {
                    continue;
                }

                //验证器不通过
                if (validator != null && !validator(faction))
                {
                    continue;
                }

                //遍历可用派系 寻找与当前派系敌对的派系
                foreach (var anotherFaction in candidateFactionList
                    .Where(anotherFaction => anotherFaction.IsFactionEffective(points, pawnGroupKindDef))
                    .Where(anotherFaction => faction.HostileTo(anotherFaction))
                    .Where(anotherFaction => validator == null || validator(anotherFaction))
                )
                {
                    faction1 = faction;
                    faction2 = anotherFaction;
                    return;
                }
            }
        }
    }
}
