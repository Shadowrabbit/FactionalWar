// ******************************************************************
//       /\ /|       @file       TriggerBecameNonHostileToFaction.cs
//       \ V/        @brief      触发器 对指定派系转变为非敌对
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 12:29:28
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse.AI.Group;

namespace SR.ModRimworld.FactionalWar
{
    public class TriggerBecameNonHostileToFaction : Trigger
    {
        private readonly Faction _targetFaction; //目标派系

        public TriggerBecameNonHostileToFaction(Faction targetFaction)
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
            if (signal.type != TriggerSignalType.FactionRelationsChanged)
            {
                return false;
            }

            if (signal.faction == null)
            {
                return false;
            }

            if (signal.faction != _targetFaction)
            {
                return false;
            }

            var previousRelationKind = signal.previousRelationKind;
            return (previousRelationKind.GetValueOrDefault() == FactionRelationKind.Hostile &
                    previousRelationKind != null) && lord.faction != null &&
                   !lord.faction.HostileTo(_targetFaction);
        }
    }
}