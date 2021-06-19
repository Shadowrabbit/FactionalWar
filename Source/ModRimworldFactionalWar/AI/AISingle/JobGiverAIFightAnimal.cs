// ******************************************************************
//       /\ /|       @file       JobGiverAIFightAnimal.cs
//       \ V/        @brief      行为节点 攻击动物
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 12:59:15
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
    public class JobGiverAIFightAnimal : JobGiver_AIFightEnemies
    {
        private const float MinTargetRequireHealthScale = 1.7f; //健康缩放最小需求 用来判断动物强度

        /// <summary>
        /// 尝试分配工作
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected override Job TryGiveJob(Pawn pawn)
        {
            //找队长旁边的动物
            var enemyTarget = FindTargetAnimal(pawn);
            if (enemyTarget == null)
            {
                return null;
            }

            //视野外看不见
            if (enemyTarget.IsInvisible())
            {
                return null;
            }

            pawn.mindState.enemyTarget = enemyTarget;
            var allowManualCastWeapons = !pawn.IsColonist;
            //获取攻击动作
            var attackVerb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (attackVerb == null)
            {
                return null;
            }

            //近战的情况
            if (attackVerb.verbProps.IsMeleeAttack)
            {
                return MeleeAttackJob(enemyTarget);
            }

            //远程的情况 先算当前位置适不适合攻击
            var num1 = (double) CoverUtility.CalculateOverallBlockChance((LocalTargetInfo) pawn,
                enemyTarget.Position, pawn.Map) > 0.00999999977648258
                ? 1
                : 0;
            var flag1 = pawn.Position.Standable(pawn.Map) &&
                        pawn.Map.pawnDestinationReservationManager.CanReserve(pawn.Position, pawn, pawn.Drafted);
            var flag2 = attackVerb.CanHitTarget(enemyTarget);
            var flag3 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
            var num2 = flag1 ? 1 : 0;
            if ((num1 & num2 & (flag2 ? 1 : 0)) != 0 || flag3 & flag2)
            {
                return MakeRangeAttackJob(enemyTarget, attackVerb);
            }

            //寻找掩体位置
            if (!TryFindShootingPosition(pawn, out var dest))
            {
                return null;
            }

            //角色已经在掩体位置了 开枪射击
            if (dest == pawn.Position)
            {
                return MakeRangeAttackJob(enemyTarget, attackVerb);
            }

            //走向掩体
            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, dest);
            job.expiryInterval = ExpiryInterval_ShooterSucceeded.RandomInRange;
            job.checkOverrideOnExpire = true;
            return job;
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <param name="resolve"></param>
        /// <returns></returns>
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiverAIFightHostileFaction = (JobGiverAIFightAnimal) base.DeepCopy(resolve);
            return jobGiverAIFightHostileFaction;
        }

        /// <summary>
        /// 寻找目标动物
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        private static Pawn FindTargetAnimal(Pawn pawn)
        {
            //集群AI
            var lord = pawn.GetLord();
            //没有成员
            if (lord.ownedPawns.Count < 1)
            {
                return null;
            }

            //队长
            var leader = lord.ownedPawns[0];

            //验证器
            bool SpoilValidator(Thing t) => (pawn == null || pawn.CanReserve(t)) && (t is Pawn animal)
                && animal.RaceProps.Animal && !animal.Downed && !animal.Dead &&
                animal.RaceProps.baseHealthScale >= MinTargetRequireHealthScale;

            //找队长身边最近的动物
            var targetThing = GenClosest.ClosestThing_Global_Reachable(leader.Position, leader.Map,
                leader.Map.mapPawns.AllPawnsSpawned, PathEndMode.ClosestTouch,
                TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), validator: SpoilValidator);

            return (Pawn) targetThing;
        }

        /// <summary>
        /// 创建远程攻击工作
        /// </summary>
        /// <param name="enemyTarget"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        private static Job MakeRangeAttackJob(Thing enemyTarget, Verb verb)
        {
            var rangeAttackJob = JobMaker.MakeJob(RimWorld.JobDefOf.AttackStatic);
            rangeAttackJob.verbToUse = verb;
            rangeAttackJob.targetA = enemyTarget;
            rangeAttackJob.endIfCantShootInMelee = true;
            rangeAttackJob.endIfCantShootTargetFromCurPos = true;
            return rangeAttackJob;
        }
    }
}