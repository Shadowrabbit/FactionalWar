// ******************************************************************
//       /\ /|       @file       PawnSpawnUtil.cs
//       \ V/        @brief      角色生成工具
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-16 23:19:15
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class PawnSpawnUtil : BaseSingleTon<PawnSpawnUtil>
    {
        public static void SpawnPawns(IReadOnlyList<Pawn> pawns, IncidentParms incidentParms, Map map, int radius)
        {
            if (!RCellFinder.TryFindRandomPawnEntryCell(out incidentParms.spawnCenter, map,
                CellFinder.EdgeRoadChance_Hostile))
            {
                return;
            }

            var stageLoc = RCellFinder.FindSiegePositionFrom(
                incidentParms.spawnCenter.IsValid ? incidentParms.spawnCenter : pawns[0].PositionHeld, map);
            var spawnRotation = Rot4.FromAngleFlat((map.Center - stageLoc).AngleFlat);
            foreach (var pawn in pawns)
            {
                var loc = CellFinder.RandomClosewalkCellNear(stageLoc, map, radius);
                GenSpawn.Spawn(pawn, loc, map, spawnRotation);
            }
        }
    }
}