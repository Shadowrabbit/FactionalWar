// ******************************************************************
//       /\ /|       @file       JobGiverAIFightHostileFaction.cs
//       \ V/        @brief      行为节点 攻击敌对派系
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 20:59:27
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
    public class JobGiverAIFightHostileFaction : JobGiver_AIFightEnemies
    {
        /// <summary>
        /// 额外的目标校验器
        /// </summary>
        /// <param name="pawn">搜寻者</param>
        /// <param name="target">目标</param>
        /// <returns></returns>
        protected override bool ExtraTargetValidator(Pawn pawn, Thing target)
        {
            //目标不是角色
            if (!(target is Pawn targetPawn))
            {
                return false;
            }

            //获取集群AI
            var lordSeacher = pawn.GetLord();
            if (!(lordSeacher.LordJob is LordJobFactionPairBase lordJobSeacher))
            {
                return false;
            }

            //目标派系是冲突派系
            return targetPawn.Faction == lordJobSeacher.TargetFaction;
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <param name="resolve"></param>
        /// <returns></returns>
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiverAIFightHostileFaction = (JobGiverAIFightHostileFaction) base.DeepCopy(resolve);
            return jobGiverAIFightHostileFaction;
        }
    }
}