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
    public class LordJobRaidFactionFirst : LordJob
    {
        private Faction _assaulterFaction; //突击者派系
        private Faction _targetFaction; //目标派系

        public LordJobRaidFactionFirst()
        {
        }

        public LordJobRaidFactionFirst(Faction assaulterFaction, Faction targetFaction)
        {
            _targetFaction = targetFaction;
            _assaulterFaction = assaulterFaction;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            Scribe_References.Look(ref _assaulterFaction, "_assaulterFaction");
            Scribe_References.Look(ref _targetFaction, "_targetFaction");
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
            var triggerFactionAssaultVictory = new TriggerFactionAssaultVictory(_targetFaction);
            transitionAssaultFactionFirstToKillHostileFactionMember.AddTrigger(triggerFactionAssaultVictory);
            transitionAssaultFactionFirstToKillHostileFactionMember.AddPreAction(new TransitionAction_Message(
                "SrClearBattlefiled".Translate(
                    (NamedArgument) _assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) _assaulterFaction.Name,
                    (NamedArgument) _targetFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) _targetFaction.Name)));
            stateGraph.AddTransition(transitionAssaultFactionFirstToKillHostileFactionMember);
            //受到玩家攻击(被激怒) 攻击对方派系 转变为 攻击敌人
            var transitionAssaultFactionFirstToAssaultEnemy =
                new Transition(lordToilAssaultFactionFirst, lordToilAssaultEnemey);
            var triggerGetDamageFromPlayer = new TriggetGetDamageFromPlayer();
            transitionAssaultFactionFirstToAssaultEnemy.AddTrigger(triggerGetDamageFromPlayer);
            transitionAssaultFactionFirstToAssaultEnemy.AddPreAction(new TransitionAction_Message(
                "SrIrritateFaction".Translate(
                    (NamedArgument) _assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) _assaulterFaction.Name)));
            stateGraph.AddTransition(transitionAssaultFactionFirstToAssaultEnemy);
            //派系和好 攻击对方派系 转变为 离开地图
            var transitionAssaultFactionFirstToExitMap = new Transition(lordToilAssaultFactionFirst, lordToilExitMap);
            var triggerBecameNonHostileToFaction = new TriggerBecameNonHostileToFaction(_targetFaction);
            transitionAssaultFactionFirstToExitMap.AddTrigger(triggerBecameNonHostileToFaction);
            transitionAssaultFactionFirstToExitMap.AddPreAction(new TransitionAction_Message(
                "MessageRaidersLeaving".Translate(
                    (NamedArgument) _assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) _assaulterFaction.Name)));
            stateGraph.AddTransition(transitionAssaultFactionFirstToExitMap);
            return stateGraph;
        }
    }
}