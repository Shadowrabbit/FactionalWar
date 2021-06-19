// ******************************************************************
//       /\ /|       @file       TriggerFactionAssaultVictory.cs
//       \ V/        @brief      触发器 派系突击胜利
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 15:17:55
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class TriggerFactionAssaultVictory : Trigger
    {
        private readonly Faction _targetFaction; //目标派系
        private const int CheckEveryTicks = 600;

        public TriggerFactionAssaultVictory(Faction targetFaction)
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

            //目标派系全部成员倒地
            return !lord.Map.mapPawns
                .SpawnedPawnsInFaction(_targetFaction).Any(pawn => !pawn.Downed && !pawn.Dead);
        }
    }
}