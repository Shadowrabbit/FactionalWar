// ******************************************************************
//       /\ /|       @file       LordToilClearBattlefield.cs
//       \ V/        @brief      集群AI流程 派系胜利清理战场
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-16 09:30:04
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilClearBattlefield : LordToil
    {
        /// <summary>
        /// 更新职责
        /// </summary>
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.SrClearBattlefield);
            }
        }
    }
}