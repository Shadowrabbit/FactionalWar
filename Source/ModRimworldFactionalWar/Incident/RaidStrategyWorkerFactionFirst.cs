﻿// ******************************************************************
//       /\ /|       @file       RaidStrategyWorkerFactionFirst.cs
//       \ V/        @brief      袭击策略 派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-13 00:14:08
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimworld.FactionalWar
{
    [UsedImplicitly]
    public class RaidStrategyWorkerFactionFirst : RaidStrategyWorker_StageThenAttack
    {
        public Faction TempTargetFaction { get; set; } //目标派系

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
            var stageLoc = RCellFinder.FindSiegePositionFrom_NewTemp(
                parms.spawnCenter.IsValid ? parms.spawnCenter : pawns[0].PositionHeld, map);
            return new LordJobStageThenAssaultFactionFirst(parms.faction, stageLoc, raidSeed, TempTargetFaction);
        }
    }
}