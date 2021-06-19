// ******************************************************************
//       /\ /|       @file       LordToilTakeWoodExit.cs
//       \ V/        @brief      集群AI流程 带着木材离开
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 23:37:55
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilTakeWoodExit : LordToil
    {
        /// <summary>
        /// 更新职责
        /// </summary>
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.SrTakeWoodExit);
            }
        }
    }
}