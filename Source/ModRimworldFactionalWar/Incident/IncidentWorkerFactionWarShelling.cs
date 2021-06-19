// ******************************************************************
//       /\ /|       @file       IncidentWorkerFactionWarShelling.cs
//       \ V/        @brief      事件 派系炮击战
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:44:31
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class IncidentWorkerFactionWarShelling : IncidentWorkerFactionWar
    {
        /// <summary>
        /// 是否可以生成事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error("target must be a map.");
                return false;
            }

            //候选派系列表
            var candidateFactionList = CandidateFactions(map).ToList();
            //需要存在两个互相敌对的派系
            return (from faction in candidateFactionList
                where faction.def.canSiege
                from anotherFaction in candidateFactionList
                where faction.HostileTo(anotherFaction)
                select anotherFaction).Any(anotherFaction => anotherFaction.def.canSiege);
        }

        /// <summary>
        /// 派系能否成为资源组
        /// </summary>
        /// <param name="f">派系</param>
        /// <param name="map">地图</param>
        /// <param name="desperate">绝望难度</param>
        /// <returns></returns>
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return base.FactionCanBeGroupSource(f, map, desperate) && f.def.canSiege;
        }

        /// <summary>
        /// 解决突袭策略
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="groupKind"></param>
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = RaidStrategyDefOf.SrShellingFaction;
        }
        
    }
}