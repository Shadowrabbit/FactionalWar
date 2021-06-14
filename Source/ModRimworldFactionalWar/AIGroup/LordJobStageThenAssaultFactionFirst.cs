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

namespace SR.ModRimworld.FactionalWar
{
    public class LordJobStageThenAssaultFactionFirst : LordJob
    {
        private const int MinTickLimit = 0x1388; //最小等待tick
        private const int MamTickLimit = 0x3A98; //最大等待tick
        private Faction _faction; //派系
        private Faction _targetFaction; //敌对派系
        private IntVec3 _stageLoc; //集结中心
        private int _raidSeed; //袭击种子

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
            Scribe_References.Look(ref _faction, "faction");
            Scribe_References.Look(ref _targetFaction, "faction");
            Scribe_Values.Look(ref _stageLoc, "stageLoc");
            Scribe_Values.Look(ref _raidSeed, "raidSeed");
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            //复制子状态机流程
            var stateGraphAssaultFactionFirst = new LordJobAssaultFactionFirst(_targetFaction, _faction).CreateGraph();
            stateGraph.AttachSubgraph(stateGraphAssaultFactionFirst);
            //子状态机初始流程
            var subGraphStartingToil = stateGraphAssaultFactionFirst.StartingToil;
            //集结 防卫状态
            var lordToilStage = new LordToil_Stage(_stageLoc);
            //集结 转变为 进攻
            var transition = new Transition(lordToilStage, subGraphStartingToil);
            var tickLimit = Rand.RangeSeeded(MinTickLimit, MamTickLimit, _raidSeed);
            transition.AddTrigger(new Trigger_TicksPassed(tickLimit));
            transition.AddTrigger(new Trigger_FractionPawnsLost(0.3f));
            transition.AddPreAction(new TransitionAction_Message(
                "SrFactionAssaultBegin".Translate(_faction.def.pawnsPlural.CapitalizeFirst(),
                    _faction.Name, _targetFaction.Name), MessageTypeDefOf.ThreatSmall, $"xxx-{_raidSeed}"));
            //唤醒成员
            transition.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transition);
            stateGraph.transitions
                .Find(x => x.triggers.Any(y => y is Trigger_BecameNonHostileToPlayer))
                .AddSource(lordToilStage);
            //设置初始状态
            stateGraph.StartingToil = lordToilStage;
            return stateGraph;
        }
    }
}