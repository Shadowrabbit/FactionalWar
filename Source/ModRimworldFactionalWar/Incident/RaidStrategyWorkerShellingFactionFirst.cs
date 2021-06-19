// ******************************************************************
//       /\ /|       @file       RaidStrategyWorkerShellingFactionFirst.cs
//       \ V/        @brief      炮击派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:31:53
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class RaidStrategyWorkerShellingFactionFirst : RaidStrategyWorkerFactionFirst
    {
        /// <summary>
        /// 创建集群AI
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="map"></param>
        /// <param name="pawns"></param>
        /// <param name="raidSeed"></param>
        /// <returns></returns>
        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        {
            var siegeSpot =
                RCellFinder.FindSiegePositionFrom_NewTemp(
                    parms.spawnCenter.IsValid ? parms.spawnCenter : pawns[0].PositionHeld, map);
            var num = parms.points * Rand.Range(0.2f, 0.3f);
            if (num < 60f)
            {
                num = 60f;
            }

            return new LordJobShellFactionFirst(parms.faction, TempTargetFaction, siegeSpot, num);
        }

        /// <summary>
        /// 能否使用
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="groupKind"></param>
        /// <returns></returns>
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return base.CanUseWith(parms, groupKind) && parms.faction.def.canSiege;
        }
    }
}