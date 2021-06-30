// ******************************************************************
//       /\ /|       @file       LordJobDefendPoint.cs
//       \ V/        @brief      集群AI 守卫某个点
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-26 22:48:49
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobDefendPoint : LordJob_DefendPoint
    {
        private IntVec3 _point;
        private float? _wanderRadius;
        public override bool IsCaravanSendable => true;
        public override bool AddFleeToil => false;

        public LordJobDefendPoint()
        {
        }

        public LordJobDefendPoint(IntVec3 point, float? wanderRadius = null)
        {
            _point = point;
            _wanderRadius = wanderRadius;
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            var lordToilDefendPoint = new LordToilDefendPoint(_point, wanderRadius: _wanderRadius);
            stateGraph.AddToil(lordToilDefendPoint);
            //添加流程 攻击敌人
            var lordToilAssaultEnemey = new LordToil_AssaultColony();
            stateGraph.AddToil(lordToilAssaultEnemey);
            //受到玩家攻击(被激怒) 攻击对方派系 转变为 攻击敌人
            var transitionDefendPointToAssaultEnemy =
                new Transition(lordToilDefendPoint, lordToilAssaultEnemey);
            var triggerGetDamageFromPlayer = new TriggetGetDamageFromPlayer();
            transitionDefendPointToAssaultEnemy.AddTrigger(triggerGetDamageFromPlayer);
            stateGraph.AddTransition(transitionDefendPointToAssaultEnemy);
            return stateGraph;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref _point, "_point");
            Scribe_Values.Look(ref _wanderRadius, "_wanderRadius");
        }
    }
}
