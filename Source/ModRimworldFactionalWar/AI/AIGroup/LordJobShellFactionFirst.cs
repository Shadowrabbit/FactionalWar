// ******************************************************************
//       /\ /|       @file       LordJobShellFactionFirst.cs
//       \ V/        @brief      集群AI 炮击派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 21:43:20
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobShellFactionFirst : LordJobFactionPairBase
    {
        private float _blueprintPoints; //蓝图数量

        public LordJobShellFactionFirst()
        {
        }

        public LordJobShellFactionFirst(Faction faction, Faction targetFaction, IntVec3 stageLoc,
            float blueprintPoints) : base(faction, targetFaction, stageLoc)
        {
            _blueprintPoints = blueprintPoints;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _blueprintPoints, "_blueprintPoints");
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            //复制状态机 旅行
            var lordJobTravel = new LordJob_Travel(stageLoc).CreateGraph();
            var subGraphTravel = stateGraph.AttachSubgraph(lordJobTravel);
            var lordToilTravel = subGraphTravel.StartingToil;
            //复制状态机 突击
            var lordJobAssaultFactionFirst = new LordJobAssaultFactionFirst(faction, targetFaction).CreateGraph();
            var subGraphAssaultFactionFirst = stateGraph.AttachSubgraph(lordJobAssaultFactionFirst);
            var lordToilAssaultFactionFirst = subGraphAssaultFactionFirst.StartingToil;
            //添加流程 派系炮击
            var lordToilSiege = new LordToilShell(stageLoc, _blueprintPoints);
            stateGraph.AddToil(lordToilSiege);
            //添加流程 离开地图
            var lordToilExitMap =
                new LordToil_ExitMap(LocomotionUrgency.Jog, interruptCurrentJob: true) {useAvoidGrid = true};
            stateGraph.AddToil(lordToilExitMap);
            //防卫转变为击杀
            var lordToilDefendPoint = subGraphTravel.FindToil<LordToil_DefendPoint>();
            var lordToilKillHostileFactionMember = subGraphAssaultFactionFirst.FindToil<LordToilKillHostileFactionMember>();
            var triggerFactionAssaultVictory = new TriggerFactionAssaultVictory(targetFaction);
            var transitionTravelToKillHostileFactionMember =
                new Transition(lordToilDefendPoint, lordToilKillHostileFactionMember);
            transitionTravelToKillHostileFactionMember.AddTrigger(triggerFactionAssaultVictory);
            transitionTravelToKillHostileFactionMember.AddPreAction(new TransitionAction_Message(
                "SrAssaultFactionVictory".Translate(
                    (NamedArgument) faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) faction.Name,
                    (NamedArgument) targetFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) targetFaction.Name)));
            stateGraph.AddTransition(transitionTravelToKillHostileFactionMember);
            //炮击转变为击杀
            var transitionSiegeToKillHostileFactionMember =
                new Transition(lordToilSiege, lordToilKillHostileFactionMember);
            transitionSiegeToKillHostileFactionMember.AddTrigger(triggerFactionAssaultVictory);
            transitionSiegeToKillHostileFactionMember.AddPreAction(new TransitionAction_Message(
                "SrAssaultFactionVictory".Translate(
                    (NamedArgument) faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) faction.Name,
                    (NamedArgument) targetFaction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) targetFaction.Name)));
            stateGraph.AddTransition(transitionSiegeToKillHostileFactionMember);
            //旅行转变为炮击
            var transitionTravelToSiege = new Transition(lordToilTravel, lordToilSiege);
            transitionTravelToSiege.AddTrigger(new Trigger_Memo("TravelArrived"));
            transitionTravelToSiege.AddTrigger(new Trigger_TicksPassed(5000));
            transitionTravelToSiege.AddPreAction(new TransitionAction_Message(
                "SrFactionShellBegin".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name, targetFaction.Name), MessageTypeDefOf.ThreatBig));
            stateGraph.AddTransition(transitionTravelToSiege);
            //炮击转变为突击
            var transitionSiegeToAssault = new Transition(lordToilSiege, lordToilAssaultFactionFirst);
            transitionSiegeToAssault.AddTrigger(new Trigger_Memo("NoBuilders"));
            transitionSiegeToAssault.AddTrigger(new Trigger_Memo("NoArtillery"));
            transitionSiegeToAssault.AddTrigger(new Trigger_PawnHarmed(0.005f));
            transitionSiegeToAssault.AddPreAction(new TransitionAction_Message(
                "SrFactionAssaultBegin".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name, targetFaction.Name), MessageTypeDefOf.ThreatBig));
            transitionSiegeToAssault.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transitionSiegeToAssault);
            //派系和好 攻击对方派系 转变为 离开地图
            var transitionSiegeToExitMap = new Transition(lordToilSiege, lordToilExitMap);
            var triggerBecameNonHostileToFaction = new TriggerBecameNonHostileToFaction(targetFaction);
            transitionSiegeToExitMap.AddTrigger(triggerBecameNonHostileToFaction);
            transitionSiegeToExitMap.AddPreAction(new TransitionAction_Message(
                "MessageRaidersLeaving".Translate(
                    (NamedArgument) faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) faction.Name)));
            stateGraph.AddTransition(transitionSiegeToExitMap);
            return stateGraph;
        }
    }
}