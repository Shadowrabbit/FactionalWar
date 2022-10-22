// ******************************************************************
//       /\ /|       @file       JobGiverAISapper.cs
//       \ V/        @brief      行为节点 工兵逻辑
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-27 23:51:33
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverAISapper : JobGiver_AISapper
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var intVec3 = pawn.mindState.duty.focus.Cell;
            if (intVec3.IsValid && intVec3.DistanceToSquared(pawn.Position) < 100.0 &&
                (intVec3.GetRoom(pawn.Map) == pawn.GetRoom() && intVec3.WithinRegions(pawn.Position, pawn.Map, 9,
                    TraverseMode.NoPassClosedDoors)))
            {
                pawn.GetLord().Notify_ReachedDutyLocation(pawn);
                return null;
            }

            if (!intVec3.IsValid)
            {
                if (!pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn)
                    .Where(x =>
                        !x.ThreatDisabled(pawn) && x.Thing.Faction.HostileTo(pawn.Faction) &&
                        pawn.CanReach((LocalTargetInfo) x.Thing, PathEndMode.OnCell, Danger.Deadly,
                            mode: TraverseMode.PassAllDestroyableThings)).TryRandomElement(out var result))
                    return null;
                intVec3 = result.Thing.Position;
            }

            if (!pawn.CanReach(intVec3, PathEndMode.OnCell, Danger.Deadly,
                mode: TraverseMode.PassAllDestroyableThings))
                return null;
            using (var path = pawn.Map.pathFinder.FindPath(pawn.Position, intVec3,
                TraverseParms.For(pawn, mode: TraverseMode.PassAllDestroyableThings)))
            {
                var blocker = path.FirstBlockingBuilding(out var cellBefore, pawn);
                if (blocker == null)
                {
                    return JobMaker.MakeJob(RimWorld.JobDefOf.Goto, intVec3, 500, true);
                }

                var job = DigUtility.PassBlockerJob(pawn, blocker, cellBefore, true, true);
                if (job != null)
                    return job;
            }

            return JobMaker.MakeJob(RimWorld.JobDefOf.Goto, intVec3, 500, true);
        }
    }
}