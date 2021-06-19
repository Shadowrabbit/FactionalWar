// ******************************************************************
//       /\ /|       @file       LordToilAssaultFactionFirst.cs
//       \ V/        @brief      集群AI流程 突击派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 11:33:22
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilAssaultFactionFirst : LordToil
    {
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.SrAssaultFactionFirst);
            }
        }
    }
}