// ******************************************************************
//       /\ /|       @file       SiteTempCamp.cs
//       \ V/        @brief      场地 临时营地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 18:08:09
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

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
        private static readonly IntRange ThreatPoints = new IntRange(3000, 5000);
        private const int Radius = 10;

        /// <summary>
        /// 地图生成后回调
        /// </summary>
        public override void PostMapGenerate()
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
            var lordJob = new LordJobRaidFactionFirst(raidFaction, Faction);
            LordMaker.MakeNewLord(Faction.OfMechanoids, lordJob, Map, pawnList);
            //生成袭击者
            foreach (var pawn in pawnList)
            {
                var loc = CellFinder.RandomClosewalkCellNear(Map.Center, Map, Radius);
                GenSpawn.Spawn(pawn, loc, Map);
            }
            //空投到中心
            DropPodUtility.DropThingsNear(Map.Center, Map, pawnList);
            //第2,3队
            for (var i = 0; i < 2; i++)
            {
                var pawnList2 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
                var lordJob2 = new LordJobRaidFactionFirst(raidFaction, Faction);
                LordMaker.MakeNewLord(raidFaction, lordJob2, Map, pawnList2);
                var near = DropCellFinder.FindRaidDropCenterDistant_NewTemp(Map);
                foreach (var pawn in pawnList2)
                {
                    var loc = CellFinder.RandomClosewalkCellNear(Map.Center, Map, Radius);
                    GenSpawn.Spawn(pawn, loc, Map);
                }
                DropPodUtility.DropThingsNear(near, Map, pawnList2);
            }
        }
    }
}
