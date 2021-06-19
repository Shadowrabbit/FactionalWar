// ******************************************************************
//       /\ /|       @file       LordJobLogging.cs
//       \ V/        @brief      集群AI 伐木
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 23:45:54
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobLogging : LordJob
    {
        private static readonly IntRange ExitTime = new IntRange(13000, 19000); //离开时间

        public override StateGraph CreateGraph()
        {
            //集群AI流程状态机
            var stateGraph = new StateGraph();
            //添加流程 伐木
            var lordToilLogging = new LordToilLogging();
            stateGraph.AddToil(lordToilLogging);
            //添加流程 带着木材离开
            var lordToilTakeWoodExit = new LordToilTakeWoodExit();
            stateGraph.AddToil(lordToilTakeWoodExit);
            //设置初始状态
            stateGraph.StartingToil = lordToilLogging;
            var faction = lord.faction;
            //过渡 偷猎到带着猎物离开
            var transitionLoggingToTakeWoodExit = new Transition(lordToilLogging, lordToilTakeWoodExit);
            var triggerTicksPassed = new Trigger_TicksPassed(ExitTime.RandomInRange);
            transitionLoggingToTakeWoodExit.AddTrigger(triggerTicksPassed);
            transitionLoggingToTakeWoodExit.AddPreAction(new TransitionAction_Message(
                "SrTakeWoodExit".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name), MessageTypeDefOf.ThreatSmall));
            stateGraph.AddTransition(transitionLoggingToTakeWoodExit);
            return stateGraph;
        }
    }
}