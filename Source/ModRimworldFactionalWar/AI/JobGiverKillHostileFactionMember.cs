// ******************************************************************
//       /\ /|       @file       JobGiverKillHostileFactionMember.cs
//       \ V/        @brief      击杀敌对派系成员
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-16 12:55:07
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using System.Linq;
using JetBrains.Annotations;
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
            //存在存活的敌对派系成员就近战击杀
            foreach (var job in from targetPawn in pawn.Map.mapPawns.PawnsInFaction(targetFaction)
                where !targetPawn.Dead
                select JobMaker.MakeJob(JobDefOf.SrKillMelee, targetPawn))
            {
                job.maxNumMeleeAttacks = 1;
                job.expiryInterval = 200;
                job.reactingToMeleeThreat = true;
                //强制击杀倒地目标 不激活的话角色不会攻击倒地目标
                job.killIncappedTarget = true;
                return job;
            }
            return null;
        }
    }
}
