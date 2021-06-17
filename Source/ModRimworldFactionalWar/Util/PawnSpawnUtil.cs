// ******************************************************************
//       /\ /|       @file       PawnSpawnUtil.cs
//       \ V/        @brief      角色生成工具
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 22:52:37
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public static class PawnSpawnUtil
    {
        /// <summary>
        /// 生成角色列表
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="pawnGroupKindDef"></param>
        /// <returns></returns>
        public static List<Pawn> SpawnPawns(IncidentParms parms, PawnGroupKindDef pawnGroupKindDef)
        {
            var target = (Map) parms.target;
            var list = PawnGroupMakerUtility
                .GeneratePawns(
                    IncidentParmsUtility.GetDefaultPawnGroupMakerParms(pawnGroupKindDef, parms,
                        true), false).ToList();
            foreach (var newThing in list)
            {
                GenSpawn.Spawn(newThing, CellFinder.RandomClosewalkCellNear(parms.spawnCenter, target, 5), target);
            }

            return list;
        }
    }
}