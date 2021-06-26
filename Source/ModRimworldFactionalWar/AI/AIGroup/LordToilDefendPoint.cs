// ******************************************************************
//       /\ /|       @file       LordToilDefendPoint.cs
//       \ V/        @brief      防守据点
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-26 20:51:06
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilDefendPoint : LordToil_DefendPoint
    {
        public LordToilDefendPoint(bool canSatisfyLongNeeds = true) : base(canSatisfyLongNeeds)
        {
        }

        public LordToilDefendPoint(IntVec3 defendPoint, float defendRadius = 28, float? wanderRadius = null) : base(
            defendPoint, defendRadius, wanderRadius)
        {
        }

        public override void UpdateAllDuties()
        {
            var lordToilDataDefendPoint = Data;
            foreach (var ownedPawn in lord.ownedPawns)
            {
                ownedPawn.mindState.duty = new PawnDuty(DutyDefOf.SrDefend, lordToilDataDefendPoint.defendPoint)
                {
                    focusSecond = lordToilDataDefendPoint.defendPoint,
                    radius = (double) ownedPawn.kindDef.defendPointRadius >= 0.0
                        ? ownedPawn.kindDef.defendPointRadius
                        : lordToilDataDefendPoint.defendRadius,
                    wanderRadius = lordToilDataDefendPoint.wanderRadius
                };
            }
        }
    }
}