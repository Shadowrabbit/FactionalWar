// ******************************************************************
//       /\ /|       @file       JobDriverKillMelee.cs
//       \ V/        @brief      具体工作 近战杀死
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-16 14:21:01
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class JobDriverKillMelee : JobDriver
    {
        /// <summary>
        /// 尝试预留
        /// </summary>
        /// <param name="errorOnFailed"></param>
        /// <returns></returns>
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (job.targetA.Thing is IAttackTarget attackTarget)
            {
                pawn.Map.attackTargetReservationManager.Reserve(pawn, job, attackTarget);
            }

            return true;
        }

        /// <summary>
        /// 工作流程
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var toilFollowAndMeleeAttack = Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, OnHit);
            toilFollowAndMeleeAttack.FailOnDespawnedOrNull(TargetIndex.A);
            yield return toilFollowAndMeleeAttack;
        }

        /// <summary>
        /// 击中后回调
        /// </summary>
        private void OnHit()
        {
            var thing = job.GetTarget(TargetIndex.A).Thing;
            pawn.meleeVerbs.TryMeleeAttack(thing, job.verbToUse);
        }
    }
}