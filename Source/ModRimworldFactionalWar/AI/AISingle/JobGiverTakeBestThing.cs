// ******************************************************************
//       /\ /|       @file       JobGiverTakeBestThing.cs
//       \ V/        @brief      行为节点 带走最好的物品
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 12:53:57
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
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
        private const float ItemsSearchRadiusOngoing = 99f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!RCellFinder.TryFindBestExitSpot(pawn, out var spot))
                return null;
            if (!StealAIUtility.TryFindBestItemToSteal(pawn.Position, pawn.Map, ItemsSearchRadiusOngoing, out var thing, pawn) ||
                GenAI.InDangerousCombat(pawn))
                return null;
            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Steal);
            job.targetA = (LocalTargetInfo)thing;
            job.targetB = spot;
            job.count = Mathf.Min(thing.stackCount,
                (int)(pawn.GetStatValue(StatDefOf.CarryingCapacity) / (double)thing.def.VolumePerUnit));
            return job;
        }
    }
}
