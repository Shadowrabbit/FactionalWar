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
    public class LordJobStageThenAssaultFactionFirst : LordJobFactionPairBase
    {
        private const int TickLimit = 0xBB8; //等待tick
        private int _raidSeed; //袭击种子

        public LordJobStageThenAssaultFactionFirst()
        {
        }

        public LordJobStageThenAssaultFactionFirst(Faction faction, Faction targetFaction, IntVec3 stageLoc,
            int raidSeed) : base(faction, targetFaction, stageLoc)
        {
            _raidSeed = raidSeed;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _raidSeed, "_raidSeed");
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            //添加流程 集结
            var lordToilStage = new LordToil_Stage(stageLoc);
            stateGraph.AddToil(lordToilStage);
            //复制子状态机 突击派系
            var stateGraphAssaultFactionFirst =
                new LordJobAssaultFactionFirst(faction, targetFaction).CreateGraph();
            stateGraph.AttachSubgraph(stateGraphAssaultFactionFirst);
            //集结 转变为 进攻
            var transition = new Transition(lordToilStage, stateGraphAssaultFactionFirst.StartingToil);
            transition.AddTrigger(new Trigger_TicksPassed(TickLimit));
            transition.AddTrigger(new Trigger_FractionPawnsLost(0.3f));
            transition.AddPreAction(new TransitionAction_Message(
                "SrFactionAssaultBegin".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name, targetFaction.Name), MessageTypeDefOf.ThreatBig, $"xxx-{_raidSeed}"));
            //唤醒成员
            transition.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transition);
            return stateGraph;
        }
    }
}