// ******************************************************************
//       /\ /|       @file       TriggetGetDamageFromPlayer.cs
//       \ V/        @brief      触发器 受到来自玩家的伤害
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 14:40:21
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class TriggetGetDamageFromPlayer : Trigger
    {
        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if (signal.type != TriggerSignalType.PawnDamaged)
            {
                return false;
            }

            //友好派系
            if (!lord.faction.HostileTo(Faction.OfPlayer))
            {
                return false;
            }

            //煽动者不存在
            if (signal.dinfo.Instigator == null)
            {
                return false;
            }

            //不是玩家派系
            if (signal.dinfo.Instigator.Faction != Faction.OfPlayer)
            {
                return false;
            }

            //不是生物 可能是炮塔之类的
            if (!(signal.dinfo.Instigator is Pawn pawn))
            {
                return true;
            }

            //动物造成伤害不算
            return !pawn.RaceProps.Animal;
        }
    }
}