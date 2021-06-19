// ******************************************************************
//       /\ /|       @file       IncidentWorkerPoaching.cs
//       \ V/        @brief      事件 偷猎
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 13:45:19
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class IncidentWorkerPoaching : IncidentWorker_RaidEnemy
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

            return Enumerable.Any(candidateFactionList, faction => faction.HostileTo(Faction.OfPlayer));
        }

        /// <summary>
        /// 派系能否成为资源组
        /// </summary>
        /// <param name="f"></param>
        /// <param name="map"></param>
        /// <param name="desperate"></param>
        /// <returns></returns>
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return base.FactionCanBeGroupSource(f, map, desperate) && f.def.humanlikeFaction;
        }

        /// <summary>
        /// 解决突袭策略
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="groupKind"></param>
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = RaidStrategyDefOf.SrPoaching;
        }

        /// <summary>
        /// 获取信件定义
        /// </summary>
        /// <returns></returns>
        protected override LetterDef GetLetterDef()
        {
            return LetterDefOf.ThreatSmall;
        }
    }
}