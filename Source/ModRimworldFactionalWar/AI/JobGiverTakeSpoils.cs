// ******************************************************************
//       /\ /|       @file       JobGiverTakeSpoils.cs
//       \ V/        @brief      行为节点 带走战利品
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 00:18:37
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverTakeSpoils : ThinkNode_JobGiver
    {
        private const float MaxSearchDistence = 12f; //最大触发距离
        private const int MinRegions = 0xF;
        private const int MaxRegions = 0xF;

        protected override Job TryGiveJob(Pawn pawn)
        {
            //需要找到最合适的逃生出口
            if (!RCellFinder.TryFindBestExitSpot(pawn, out var spot))
            {
                return null;
            }

            //寻找身边合适的战利品
            if (!TryFindBestSpoilsToTake(pawn.Position, pawn.Map, MaxSearchDistence, out var thing, pawn) ||
                GenAI.InDangerousCombat(pawn))
            {
                return null;
            }

            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Steal);
            job.targetA = thing;
            job.targetB = spot;
            job.count = Mathf.Min(thing.stackCount,
                (int) (pawn.GetStatValue(StatDefOf.CarryingCapacity) / thing.def.VolumePerUnit));
            return job;
        }

        /// <summary>
        /// 寻找最合适的战利品
        /// </summary>
        /// <param name="root"></param>
        /// <param name="map"></param>
        /// <param name="maxDist"></param>
        /// <param name="item"></param>
        /// <param name="seacher"></param>
        /// <param name="disallowed"></param>
        /// <returns></returns>
        private static bool TryFindBestSpoilsToTake(IntVec3 root, Map map, float maxDist, out Thing item, Pawn seacher,
            ICollection<Thing> disallowed = null)
        {
            if (map == null)
            {
                item = null;
                return false;
            }

            if (seacher != null && !seacher.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                item = null;
                return false;
            }

            //搜索者存在 但无法触碰地图边界 或者 搜索者不存在
            if (seacher != null && !map.reachability.CanReachMapEdge(seacher.Position,
                TraverseParms.For(seacher, Danger.Some)) || (seacher == null && !map.reachability.CanReachMapEdge(root,
                TraverseParms.For(TraverseMode.PassDoors, Danger.Some))))
            {
                item = null;
                return false;
            }

            //获取集群AI
            var lordSeacher = seacher.GetLord();
            if (!(lordSeacher.LordJob is LordJobStageThenAssaultFactionFirst lordJobSeacher))
            {
                item = null;
                return false;
            }

            //目标派系
            var targetFaction = lordJobSeacher.TargetFaction;
            if (targetFaction == null)
            {
                item = null;
                return false;
            }

            //验证器 搜索者不存在 或者搜索者可以预留当前物体 并且没有禁用 并且物体可以被偷 并且物体没在燃烧中 并且物品周围有敌对派系尸体
            bool SpoilValidator(Thing t) => (seacher == null || seacher.CanReserve(t)) &&
                                            (disallowed == null || !disallowed.Contains(t)) && t.def.stealable &&
                                            !t.IsBurning() && IsThingNearByCorpse(t, targetFaction);

            item = GenClosest.ClosestThing_Regionwise_ReachablePrioritized(root, map,
                ThingRequest.ForGroup(ThingRequestGroup.HaulableEverOrMinifiable), PathEndMode.ClosestTouch,
                TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), maxDist, SpoilValidator,
                StealAIUtility.GetValue, MinRegions, MaxRegions);

            return item != null;
        }

        /// <summary>
        /// 物体附近是否存在敌对派系尸体
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="faction"></param>
        /// <returns></returns>
        private static bool IsThingNearByCorpse(Thing thing, Faction faction)
        {
            var posRoot = thing.Position;
            var cellRectRoot = new CellRect(posRoot.x - 1, posRoot.z - 1, 3, 3);
            Log.Warning($"起始点{posRoot.x},{posRoot.z}");
            foreach (var cell in cellRectRoot.EdgeCells)
            {
                Log.Warning($"{cell.x},{cell.z}");
                var corpse = cell.GetFirstThing<Pawn>(thing.Map);
                //没找到角色
                if (corpse == null)
                {
                    Log.Warning("1");
                    continue;
                }

                //当前角色没死
                if (!corpse.Dead)
                {
                    Log.Warning("2");
                    continue;
                }

                //不是目标派系的尸体
                if (corpse.Faction != faction)
                {
                    Log.Warning("3");
                    continue;
                }

                return true;
            }

            Log.Warning("结束");
            return false;
        }
    }
}