// ******************************************************************
//       /\ /|       @file       JobGiverKidnapFaction.cs
//       \ V/        @brief      行为节点 绑架派系
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 23:32:54
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverKidnapFaction : ThinkNode_JobGiver
    {
        public const float VictimSearchRadiusInitial = 8f;
        private const float VictimSearchRadiusOngoing = 18f;


        // Token: 0x06004B93 RID: 19347 RVA: 0x001A60D8 File Offset: 0x001A42D8
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!RCellFinder.TryFindBestExitSpot(pawn, out var c))
            {
                return null;
            }

            if (!KidnapAIUtil.TryFindGoodKidnapVictim(pawn, 18f, out var t) ||
                GenAI.InDangerousCombat(pawn))
            {
                return null;
            }

            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Kidnap);
            job.targetA = t;
            job.targetB = c;
            job.count = 1;
            return job;
        }
    }
}