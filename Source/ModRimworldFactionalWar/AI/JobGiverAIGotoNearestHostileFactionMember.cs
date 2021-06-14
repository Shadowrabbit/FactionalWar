// ******************************************************************
//       /\ /|       @file       JobGiverAIGotoNearestHostileFactionMember.cs
//       \ V/        @brief      行为节点 走向最近的敌对派系成员
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 21:57:30
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobGiverAIGotoNearestHostileFactionMember : JobGiver_AIGotoNearestHostile
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //集群AI
            var lord = pawn.GetLord();
            //当前的集群AI流程
            if (!(lord.LordJob is LordJobStageThenAssaultFactionFirst lordJob))
            {
                return null;
            }

            //目标派系
            var targetFaction = lordJob.TargetFaction;
            //保底目标
            var num = float.MaxValue;
            var targetThing = (Thing) null;
            var potentialTargetsFor =
                pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
            foreach (var target in potentialTargetsFor)
            {
                if (target.ThreatDisabled(pawn) || !AttackTargetFinder.IsAutoTargetable(target))
                {
                    continue;
                }

                //目标不是当前敌对派系
                var tempTargetThing = (Thing) target;
                if (tempTargetThing.Faction != targetFaction)
                {
                    continue;
                }

                var squared = tempTargetThing.Position.DistanceToSquared(pawn.Position);
                if (!(squared < num) || !pawn.CanReach(tempTargetThing, PathEndMode.OnCell, Danger.Deadly))
                {
                    continue;
                }

                num = squared;
                targetThing = tempTargetThing;
            }

            if (targetThing == null)
            {
                return null;
            }

            var job = JobMaker.MakeJob(JobDefOf.Goto, targetThing);
            job.checkOverrideOnExpire = true;
            job.expiryInterval = 500;
            job.collideWithPawns = true;
            return job;
        }
    }
}