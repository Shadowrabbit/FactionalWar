// ******************************************************************
//       /\ /|       @file       LordJobStageThenAssaultFactionFirst.cs
//       \ V/        @brief      集群AI 突击敌对派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-11 09:44:29
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobStageThenAssaultFactionFirst : LordJob
    {
        public Faction TargetFaction => _targetFaction;
        private const int TickLimit = 0xBB8; //等待tick
        private Faction _faction; //派系
        private Faction _targetFaction; //敌对派系
        private IntVec3 _stageLoc; //集结中心
        private int _raidSeed; //袭击种子

        public LordJobStageThenAssaultFactionFirst()
        {
        }

        public LordJobStageThenAssaultFactionFirst(Faction faction, IntVec3 stageLoc, int raidSeed,
            Faction targetFaction)
        {
            _faction = faction;
            _stageLoc = stageLoc;
            _raidSeed = raidSeed;
            _targetFaction = targetFaction;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            Scribe_References.Look(ref _faction, "_faction");
            Scribe_References.Look(ref _targetFaction, "_targetFaction");
            Scribe_Values.Look(ref _stageLoc, "_stageLoc");
            Scribe_Values.Look(ref _raidSeed, "_raidSeed");
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            //复制子状态机流程
            var stateGraphAssaultFactionFirst =
                new LordJobAssaultFactionFirst(_targetFaction, _faction).CreateGraph();
            stateGraph.AttachSubgraph(stateGraphAssaultFactionFirst);
            //子状态机初始流程
            var subGraphStartingToil = stateGraphAssaultFactionFirst.StartingToil;
            //集结 防卫状态
            var lordToilStage = new LordToil_Stage(_stageLoc);
            //集结 转变为 进攻
            var transition = new Transition(lordToilStage, subGraphStartingToil);
            transition.AddTrigger(new Trigger_TicksPassed(TickLimit));
            transition.AddTrigger(new Trigger_FractionPawnsLost(0.3f));
            transition.AddPreAction(new TransitionAction_Message(
                "SrFactionAssaultBegin".Translate(_faction.def.pawnsPlural.CapitalizeFirst(),
                    _faction.Name, _targetFaction.Name), MessageTypeDefOf.ThreatBig, $"xxx-{_raidSeed}"));
            //唤醒成员
            transition.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transition);
            //设置初始状态
            stateGraph.StartingToil = lordToilStage;
            return stateGraph;
        }
    }
}