// ******************************************************************
//       /\ /|       @file       SiteFactionWarContention.cs
//       \ V/        @brief      派系争夺战场地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 09:14:47
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
    public class SiteFactionWarContention : Site
    {
        private readonly IntRange _factionPoints = new IntRange(3000, 5000);
        private const int Radius = 8;

        /// <summary>
        /// 生成地图后回调
        /// </summary>
        public override void PostMapGenerate()
        {
            bool Validator(Faction faction) =>
                (faction.def.techLevel >= TechLevel.Industrial && faction.HostileTo(Faction.OfPlayer));

            //生成两个相互敌对的派系 设置集群AI互相攻击并争夺资源
            FactionUtil.GetHostileFactionPair(out var faction1, out var faction2, _factionPoints.min,
                PawnGroupKindDefOf.Combat,
                Find.FactionManager.AllFactionsVisible.ToList(), Validator);
            if (faction1 == null || faction2 == null)
            {
                return;
            }

            //创建派系1的角色 空投到地图中心
            var incidentParms1 = new IncidentParms
                {points = _factionPoints.RandomInRange, faction = faction1, target = Map};
            var pawnGroupMakerParms1 =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms1);
            var pawnList1 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms1).ToList();
            ResolveArrive(pawnList1, incidentParms1);
            ResolveLordJob(pawnList1, faction1);
            //创建派系2的角色 空投到地图中心
            var incidentParms2 = new IncidentParms
                {points = 2 * _factionPoints.RandomInRange, faction = faction2, target = Map};
            var pawnGroupMakerParms2 =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms2);
            var pawnList2 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms2).ToList();
            ResolveArrive(pawnList2, incidentParms2);
            ResolveLordJob(pawnList2, faction2);
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

        /// <summary>
        /// 创建集群AI
        /// </summary>
        /// <param name="pawns"></param>
        /// <param name="faction"></param>
        private void ResolveLordJob(IEnumerable<Pawn> pawns, Faction faction)
        {
            var lordJobFactionContention = new LordJobFactionContention(Map.Center);
            LordMaker.MakeNewLord(faction, lordJobFactionContention, Map, pawns);
        }
    }
}
