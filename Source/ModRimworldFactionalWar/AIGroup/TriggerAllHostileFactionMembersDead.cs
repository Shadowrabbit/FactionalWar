// ******************************************************************
//       /\ /|       @file       TriggerAllHostileFactionMembersDead.cs
//       \ V/        @brief      触发器 全部敌对派系成员死亡
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-16 12:00:21
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class TriggerAllHostileFactionMembersDead : Trigger
    {
        private readonly Faction _targetFaction; //目标派系
        private const int CheckEveryTicks = 600;

        public TriggerAllHostileFactionMembersDead(Faction targetFaction)
        {
            _targetFaction = targetFaction;
        }


        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="lord"></param>
        /// <param name="signal"></param>
        /// <returns></returns>
        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if (signal.type != TriggerSignalType.Tick)
            {
                return false;
            }

            //降低触发频率 优化性能
            if (Find.TickManager.TicksGame % CheckEveryTicks != 0)
            {
                return false;
            }

            //目标派系全部成员死亡
            return !lord.Map.mapPawns
                .SpawnedPawnsInFaction(_targetFaction).Any(pawn => !pawn.Dead);
        }
    }
}