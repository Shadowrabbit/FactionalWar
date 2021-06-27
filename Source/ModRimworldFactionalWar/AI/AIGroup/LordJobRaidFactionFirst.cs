// ******************************************************************
//       /\ /|       @file       LordJobRaidFactionFirst.cs
//       \ V/        @brief      集群AI 袭击派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 23:07:53
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobRaidFactionFirst : LordJobFactionPairBase
    {
        public override bool AddFleeToil => false;

        public LordJobRaidFactionFirst()
        {
        }

        public LordJobRaidFactionFirst(Faction faction, Faction targetFaction)
        {
            this.faction = faction;
            this.targetFaction = targetFaction;
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            //集群AI流程状态机
            var stateGraph = new StateGraph();
            //添加流程 攻击对方派系
            var lordToilAssaultFactionFirst = new LordToilAssaultFactionFirst();
            stateGraph.AddToil(lordToilAssaultFactionFirst);
            //添加流程 攻击敌人
            var lordToilAssaultEnemey = new LordToil_AssaultColony();
            stateGraph.AddToil(lordToilAssaultEnemey);
            //添加流程 离开地图
            var lordToilExitMap =
                new LordToil_ExitMap(LocomotionUrgency.Jog, interruptCurrentJob: true) {useAvoidGrid = true};
            stateGraph.AddToil(lordToilExitMap);
            //添加流程 掠夺派系
            var lordToilPlunderFaction = new LordToilPlunderFaction();
            stateGraph.AddToil(lordToilPlunderFaction);
            //没有除玩家外敌对派系（派系胜利离开）攻击对方派系 转变为 掠夺
            var transitionAssaultFactionFirstToKillHostileFactionMember =
                new Transition(lordToilAssaultFactionFirst, lordToilPlunderFaction);
            var triggerFactionAssaultVictory = new TriggerFactionAssaultVictory(targetFaction);
            transitionAssaultFactionFirstToKillHostileFactionMember.AddTrigger(triggerFactionAssaultVictory);
            transitionAssaultFactionFirstToKillHostileFactionMember.AddPreAction(new TransitionAction_Message(
                "SrClearBattlefiled".Translate(
                    (NamedArgument) faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) faction.Name,
                    (NamedArgument) targetFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) targetFaction.Name)));
            stateGraph.AddTransition(transitionAssaultFactionFirstToKillHostileFactionMember);
            //受到玩家攻击(被激怒) 攻击对方派系 转变为 攻击敌人
            var transitionAssaultFactionFirstToAssaultEnemy =
                new Transition(lordToilAssaultFactionFirst, lordToilAssaultEnemey);
            var triggerGetDamageFromPlayer = new TriggetGetDamageFromPlayer();
            transitionAssaultFactionFirstToAssaultEnemy.AddTrigger(triggerGetDamageFromPlayer);
            transitionAssaultFactionFirstToAssaultEnemy.AddPreAction(new TransitionAction_Message(
                "SrIrritateFaction".Translate(
                    (NamedArgument) faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) faction.Name)));
            stateGraph.AddTransition(transitionAssaultFactionFirstToAssaultEnemy);
            //派系和好 攻击对方派系 转变为 离开地图
            var transitionAssaultFactionFirstToExitMap = new Transition(lordToilAssaultFactionFirst, lordToilExitMap);
            var triggerBecameNonHostileToFaction = new TriggerBecameNonHostileToFaction(targetFaction);
            transitionAssaultFactionFirstToExitMap.AddTrigger(triggerBecameNonHostileToFaction);
            transitionAssaultFactionFirstToExitMap.AddPreAction(new TransitionAction_Message(
                "MessageRaidersLeaving".Translate(
                    (NamedArgument) faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) faction.Name)));
            stateGraph.AddTransition(transitionAssaultFactionFirstToExitMap);
            return stateGraph;
        }
    }
}