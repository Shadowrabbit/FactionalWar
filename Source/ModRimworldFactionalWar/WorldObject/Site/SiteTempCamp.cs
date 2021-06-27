// ******************************************************************
//       /\ /|       @file       SiteTempCamp.cs
//       \ V/        @brief      场地 临时营地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 18:08:09
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class SiteTempCamp : Site
    {
        private static readonly IntRange ThreatPoints = new IntRange(3000, 8000);
        private const int Radius = 10;

        /// <summary>
        /// 地图生成后回调
        /// </summary>
        public override void PostMapGenerate()
        {
            SpawnRaider();
        }

        /// <summary>
        /// 生成袭击者
        /// </summary>
        private void SpawnRaider()
        {
            var points = ThreatPoints.RandomInRange;

            //创建袭击者
            bool Validator(Faction faction) => (faction.IsFactionEffective(points, PawnGroupKindDefOf.Combat));
            var raidFaction = Faction.FindHostileFaction(Validator);
            var incidentParms = new IncidentParms {points = points, faction = raidFaction, target = Map};
            var pawnGroupMakerParms =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms);
            //用默认角色组生成器
            var pawnList = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            ResolveArrive(pawnList, incidentParms);
            ResolveLordJob(pawnList, raidFaction, Faction);
        }

        /// <summary>
        /// 创建集群AI
        /// </summary>
        /// <param name="pawns"></param>
        /// <param name="assaultFaction"></param>
        /// <param name="targetFaction"></param>
        /// <returns></returns>
        private void ResolveLordJob(IEnumerable<Pawn> pawns, Faction assaultFaction,
            Faction targetFaction)
        {
            var lordJobShellFactionFirst = new LordJobRaidFactionFirst(assaultFaction, targetFaction);
            LordMaker.MakeNewLord(assaultFaction, lordJobShellFactionFirst, Map, pawns);
        }

        /// <summary>
        /// 解决入场
        /// </summary>
        private void ResolveArrive(IEnumerable<Pawn> pawns, IncidentParms incidentParms)
        {
            if (!RCellFinder.TryFindRandomPawnEntryCell(out incidentParms.spawnCenter, Map,
                CellFinder.EdgeRoadChance_Hostile))
            {
                return;
            }

            var spawnRotation = Rot4.FromAngleFlat((Map.Center - incidentParms.spawnCenter).AngleFlat);
            foreach (var pawn in pawns)
            {
                var loc = CellFinder.RandomClosewalkCellNear(incidentParms.spawnCenter, Map, Radius);
                GenSpawn.Spawn(pawn, loc, Map, spawnRotation);
            }
        }
    }
}