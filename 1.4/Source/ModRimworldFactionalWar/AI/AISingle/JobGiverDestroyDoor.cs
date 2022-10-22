// ******************************************************************
//       /\ /|       @file       JobGiverDestroyDoor.cs
//       \ V/        @brief      行为节点 破坏所有的门
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-27 11:38:08
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverDestroyDoor : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //最近的目标
            var targetThing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.NoPassClosedDoors),
                validator: (t) => t.Faction.HostileTo(pawn.Faction) && t is Building building && building.def.IsDoor);
            if (targetThing == null)
            {
                return null;
            }

            var job = JobMaker.MakeJob(JobDefOf.SrKillMelee, targetThing);
            job.maxNumMeleeAttacks = 1;
            job.expiryInterval = 200;
            job.reactingToMeleeThreat = true;
            //强制击杀倒地目标 不激活的话角色不会攻击倒地目标
            job.killIncappedTarget = true;
            return job;
        }
    }
}