// ******************************************************************
//       /\ /|       @file       IncidentWorkerPoaching.cs
//       \ V/        @brief      事件 偷猎
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 13:45:19
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class IncidentWorkerPoaching : IncidentWorker_RaidEnemy
    {
        private const int MaxPoints = 2000; //最大事件点数
        private const int MinPoints = 200; //最小事件点数
        private const int MaxPawnCount = 5; //最大生成角色数量
        private const int MinPawnCount = 1; //最小生成角色数量

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
        /// 执行事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error("target must be a map.");
                return false;
            }
            //处理袭击点数
            ResolveRaidPoints(parms);
            //处理袭击派系
            if (!TryResolveRaidFaction(parms))
            {
                Log.Warning("cant find raid factions");
                return false;
            }
            //解决袭击策略
            ResolveRaidStrategy(parms, PawnGroupKindDefOf.Settlement);
            //解决到达方式
            ResolveRaidArriveMode(parms);
            //尝试解决袭击召唤中心
            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                Log.Warning($"cant resolve raid spawn center: {parms}");
                return false;
            }
            //根据策略再次调整袭击点数
            parms.points = AdjustedRaidPoints(parms.points, parms.raidArrivalMode,
                parms.raidStrategy, parms.faction, PawnGroupKindDefOf.Settlement);
            //生成派系部队
            var pawnListFaction1 = parms.raidStrategy.Worker.SpawnThreats(parms);
            return true;
        }

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
        /// 尝试解决突袭派系
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            var map = parms.target as Map; //目标地图
            //全部派系
            var candidateFactionList = CandidateFactions(map).ToList();
            //乱序
            candidateFactionList.Shuffle();
            return candidateFactionList.Where(faction => faction.IsFactionEffective(parms.points, PawnGroupKindDefOf.Settlement))
                .Any(faction => faction.HostileTo(Faction.OfPlayer));
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

        /// <summary>
        /// 解决袭击点数
        /// </summary>
        /// <param name="parms"></param>
        protected override void ResolveRaidPoints(IncidentParms parms)
        {
            parms.points = Mathf.Clamp(StorytellerUtility.DefaultThreatPointsNow(parms.target), MinPoints, MaxPoints);
        }


        private List<Pawn> SpawnPawns(IncidentParms parms)
        {
            var target = (Map)parms.target;
            var list = PawnGroupMakerUtility
                .GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDef, parms, true), false).ToList<Pawn>();
            foreach (var newThing in list)
                GenSpawn.Spawn(newThing, CellFinder.RandomClosewalkCellNear(parms.spawnCenter, target, 5), target);
            return list;
        }
        public PawnGroupKindDef PawnGroupKindDef { get; set; }
    }
}
