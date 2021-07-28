// ******************************************************************
//       /\ /|       @file       JobGiverAIGotoNearestHostileFactionMember.cs
//       \ V/        @brief      行为节点 走向最近的敌对派系成员
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 21:57:30
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverAIGotoNearestHostileFactionMember : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //集群AI
            var lord = pawn.GetLord();
            //当前的集群AI流程
            if (!(lord.LordJob is LordJobFactionPairBase lordJob))
            {
                return null;
            }

            //目标派系
            var targetFaction = lordJob.TargetFaction;
            //保底目标
            var num = float.MaxValue;
            var targetThing = (Thing)null;
            var potentialTargetsFor =
                pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
            foreach (var target in potentialTargetsFor)
            {
                if (target.ThreatDisabled(pawn) || !AttackTargetFinder.IsAutoTargetable(target))
                {
                    continue;
                }

                //目标不是角色
                if (!(target is Pawn targetPawn))
                {
                    continue;
                }

                //目标不是当前敌对派系
                if (targetPawn.Faction != targetFaction)
                {
                    continue;
                }

                //目标是囚犯
                if (targetPawn.IsPrisoner)
                {
                    continue;
                }

                var squared = targetPawn.Position.DistanceToSquared(pawn.Position);
                if (!(squared < num) || !pawn.CanReach(targetPawn, PathEndMode.OnCell, Danger.Deadly))
                {
                    continue;
                }

                num = squared;
                targetThing = targetPawn;
            }

            if (targetThing == null)
            {
                return null;
            }

            //走向目标
            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, targetThing);
            job.checkOverrideOnExpire = true;
            job.expiryInterval = 500;
            job.collideWithPawns = true;
            return job;
        }
    }
}
