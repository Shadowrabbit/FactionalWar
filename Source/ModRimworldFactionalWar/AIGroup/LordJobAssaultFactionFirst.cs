// ******************************************************************
//       /\ /|       @file       LordJobAssaultFactionFirst.cs
//       \ V/        @brief      集群AI 突击敌对派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-13 08:34:28
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobAssaultFactionFirst : LordJob
    {
        private Faction _assaulterFaction; //突击者派系
        private Faction _targetFaction; //目标派系

        public LordJobAssaultFactionFirst()
        {
        }

        public LordJobAssaultFactionFirst(Faction targetFaction, Faction assaulterFaction)
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
            //没有除玩家外敌对派系（派系胜利离开）攻击对方派系 转变为 离开地图
            var transitionFactionVictory = new Transition(lordToilAssaultFactionFirst, lordToilExitMap);
            var triggerFactionAssaultVictory = new TriggerFactionAssaultVictory(_targetFaction);
            transitionFactionVictory.AddTrigger(triggerFactionAssaultVictory);
            transitionFactionVictory.AddPreAction(new TransitionAction_Message(
                "SrAssaultFactionVictory".Translate(
                    (NamedArgument) _assaulterFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) _assaulterFaction.Name)));
            stateGraph.AddTransition(transitionFactionVictory);
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