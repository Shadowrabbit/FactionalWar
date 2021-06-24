// ******************************************************************
//       /\ /|       @file       LordJobFactionContention.cs
//       \ V/        @brief      集群AI 派系争夺资源
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 18:20:20
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobFactionContention : LordJob
    {
        private IntVec3 _occupyLocation; //占据中心

        public LordJobFactionContention()
        {
        }

        public LordJobFactionContention(IntVec3 occupyLocation)
        {
            _occupyLocation = occupyLocation;
        }


        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _occupyLocation, "_occupyLocation");
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        public override StateGraph CreateGraph()
        {
            //集群AI流程状态机
            var stateGraph = new StateGraph();
            //防卫状态
            var lordToilDefendPoint = new LordToil_DefendPoint(_occupyLocation);
            stateGraph.AddToil(lordToilDefendPoint);
            //撤离状态 带走队友 带走地图上价值最高的物品 离开地图
            var lordToilRetreat = new LordToilRetreat();
            stateGraph.AddToil(lordToilRetreat);
            //防卫状态转变为撤退状态
            var transitionDefendPointToRetreat = new Transition(lordToilDefendPoint, lordToilRetreat);
            transitionDefendPointToRetreat.AddTrigger(new Trigger_HighValueThingsAround());
            stateGraph.AddTransition(transitionDefendPointToRetreat);
            return stateGraph;
        }
    }
}
