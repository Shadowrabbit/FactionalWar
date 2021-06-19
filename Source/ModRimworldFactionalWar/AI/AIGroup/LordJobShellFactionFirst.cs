// ******************************************************************
//       /\ /|       @file       LordJobShellFactionFirst.cs
//       \ V/        @brief      集群AI 炮击派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 21:43:20
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobShellFactionFirst : LordJob
    {
        private Faction _faction; //派系
        private Faction _targetFaction; //敌对派系
        private IntVec3 _siegeSpot; //集结坐标
        private float _blueprintPoints; //蓝图数量

        public LordJobShellFactionFirst()
        {
        }

        public LordJobShellFactionFirst(Faction faction, Faction targetFaction, IntVec3 siegeSpot,
            float blueprintPoints)
        {
            _faction = faction;
            _targetFaction = targetFaction;
            _siegeSpot = siegeSpot;
            _blueprintPoints = blueprintPoints;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            Scribe_References.Look(ref _faction, "_faction");
            Scribe_References.Look(ref _targetFaction, "_targetFaction");
            Scribe_Values.Look(ref _siegeSpot, "_siegeSpot");
            Scribe_Values.Look(ref _blueprintPoints, "_blueprintPoints");
        }

        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            //复制状态机 旅行
            var lordJobTravel = new LordJob_Travel(_siegeSpot).CreateGraph();
            var subGraphTravel = stateGraph.AttachSubgraph(lordJobTravel);
            //复制状态机 突击
            var lordJobAssaultFactionFirst = new LordJobAssaultFactionFirst(_faction, _targetFaction).CreateGraph();
            var subGraphAssaultFactionFirst = stateGraph.AttachSubgraph(lordJobAssaultFactionFirst);
            //添加流程 派系炮击
            var lordToilSiege = new LordToil_Siege(_siegeSpot, _blueprintPoints);
            stateGraph.AddToil(lordToilSiege);
            //添加流程 离开地图
            var lordToilExitMap =
                new LordToil_ExitMap(LocomotionUrgency.Jog, interruptCurrentJob: true) {useAvoidGrid = true};
            stateGraph.AddToil(lordToilExitMap);
            //旅行转变为炮击
            var transitionTravelToSiege = new Transition(subGraphTravel.StartingToil, lordToilSiege);
            transitionTravelToSiege.AddTrigger(new Trigger_Memo("TravelArrived"));
            transitionTravelToSiege.AddTrigger(new Trigger_TicksPassed(5000));
            transitionTravelToSiege.AddPreAction(new TransitionAction_Message(
                "SrFactionShellBegin".Translate(_faction.def.pawnsPlural.CapitalizeFirst(),
                    _faction.Name, _targetFaction.Name), MessageTypeDefOf.ThreatBig));
            stateGraph.AddTransition(transitionTravelToSiege);
            //炮击转变为突击
            var transitionSiegeToAssault = new Transition(lordToilSiege, subGraphAssaultFactionFirst.StartingToil);
            transitionSiegeToAssault.AddTrigger(new Trigger_Memo("NoBuilders"));
            transitionSiegeToAssault.AddTrigger(new Trigger_Memo("NoArtillery"));
            transitionSiegeToAssault.AddTrigger(new Trigger_PawnHarmed(0.02f));
            transitionSiegeToAssault.AddTrigger(new Trigger_TicksPassed(12000));
            transitionSiegeToAssault.AddPreAction(new TransitionAction_Message(
                "SrFactionAssaultBegin".Translate(_faction.def.pawnsPlural.CapitalizeFirst(),
                    _faction.Name, _targetFaction.Name), MessageTypeDefOf.ThreatBig));
            transitionSiegeToAssault.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transitionSiegeToAssault);
            //派系和好 攻击对方派系 转变为 离开地图
            var transitionSiegeToExitMap = new Transition(lordToilSiege, lordToilExitMap);
            var triggerBecameNonHostileToFaction = new TriggerBecameNonHostileToFaction(_targetFaction);
            transitionSiegeToExitMap.AddTrigger(triggerBecameNonHostileToFaction);
            transitionSiegeToExitMap.AddPreAction(new TransitionAction_Message(
                "MessageRaidersLeaving".Translate(
                    (NamedArgument) _faction.def.pawnsPlural.CapitalizeFirst(),
                    (NamedArgument) _faction.Name)));
            stateGraph.AddTransition(transitionSiegeToExitMap);
            return stateGraph;
        }
    }
}