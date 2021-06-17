// ******************************************************************
//       /\ /|       @file       JobGiverKillHostileFactionMember.cs
//       \ V/        @brief      击杀敌对派系成员
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-16 12:55:07
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
    public class JobGiverKillHostileFactionMember : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //获取集群AI
            var lordSeacher = pawn.GetLord();
            if (!(lordSeacher.LordJob is LordJobStageThenAssaultFactionFirst lordJobSeacher))
            {
                return null;
            }

            //目标敌对派系
            var targetFaction = lordJobSeacher.TargetFaction;
            if (targetFaction == null)
            {
                return null;
            }

            //最近的目标
            var targetThing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(pawn),
                validator: (t) => TargetValidator(t, targetFaction));
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

        /// <summary>
        /// 目标验证器
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetFaction"></param>
        /// <returns></returns>
        private static bool TargetValidator(Thing target, Faction targetFaction)
        {
            Log.Warning("目标验证器");
            if (!(target is Pawn pawn))
            {
                Log.Warning("1");
                return false;
            }

            if (pawn.Faction != targetFaction)
            {
                Log.Warning("2");
                return false;
            }

            return !pawn.Dead;
        }
    }
}
