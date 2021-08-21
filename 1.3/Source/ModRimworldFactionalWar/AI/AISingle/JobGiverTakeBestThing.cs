// ******************************************************************
//       /\ /|       @file       JobGiverTakeBestThing.cs
//       \ V/        @brief      行为节点 带走最好的物品
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 12:53:57
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverTakeBestThing : ThinkNode_JobGiver
    {
        private const float MaxSearchDistence = 999f; //最大触发距离

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

            //验证器 搜索者不存在 或者搜索者可以预留当前物体 并且没有禁用 并且物体可以被偷 并且物体没在燃烧中
            bool SpoilValidator(Thing t) => (seacher == null || seacher.CanReserve(t))
                                            && (disallowed == null || !disallowed.Contains(t))
                                            && t.def.stealable
                                            && t.def.plant == null
                                            && !t.IsBurning()
                                            && !(t is Corpse);

            item = GenClosest.ClosestThing_Regionwise_ReachablePrioritized(root, map,
                ThingRequest.ForGroup(ThingRequestGroup.HaulableEverOrMinifiable), PathEndMode.ClosestTouch,
                TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), maxDist, SpoilValidator,
                StealAIUtility.GetValue);

            return item != null;
        }
    }
}