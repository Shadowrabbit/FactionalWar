// ******************************************************************
//       /\ /|       @file       LordToilLogging.cs
//       \ V/        @brief      集群AI流程 伐木
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 23:51:05
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilLogging : LordToil
    {
        /// <summary>
        /// 更新职责
        /// </summary>
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.SrLogging);
            }
        }
    }
}