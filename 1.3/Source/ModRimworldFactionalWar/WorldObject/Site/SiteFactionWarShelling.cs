// ******************************************************************
//       /\ /|       @file       SiteFactionWarShelling.cs
//       \ V/        @brief      派系炮击战地点
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-22 22:20:50
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
    public class SiteFactionWarShelling : Site
    {
        private static readonly IntRange ThreatPoints = new IntRange(3000, 9999);
        private const float MinBlueprintPoints = 60f; //最小蓝图点数
        private const int TimeOutTick = 90000;
        private const int Radius = 8; //召唤半径

        /// <summary>
        /// 生成时回调
        /// </summary>
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            var comp = GetComponent<TimeoutComp>();
            if (comp == null)
            {
                Log.Error("[SR.ModRimWorld.FactionalWar]can't find TimeoutComp in SiteFactionWarShelling");
                return;
            }

            comp.StartTimeout(TimeOutTick);
        }

        /// <summary>
        /// 生成地图后回调
        /// </summary>
        public override void PostMapGenerate()
        {
            var points = ThreatPoints.RandomInRange;

            //找到两个互相敌对的派系
            bool Validator(Faction faction) => (faction.def.techLevel >= TechLevel.Industrial);
            FactionUtil.GetHostileFactionPair(out var faction1, out var faction2, points,
                PawnGroupKindDefOf.Combat, Find.FactionManager.AllFactionsVisible.ToList(), Validator);
            //找不到
            if (faction1 == null || faction2 == null)
            {
                return;
            }

            //创建两个派系的成员
            var incidentParms1 = new IncidentParms {points = points, faction = faction1, target = Map};
            var pawnGroupMakerParms1 =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms1);
            var pawnList1 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms1);
            var incidentParms2 = new IncidentParms {points = points, faction = faction2, target = Map};
            var pawnGroupMakerParms2 =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms2);
            var pawnList2 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms2);
            //边缘入场
            PawnSpawnUtil.SpawnPawns(pawnList1, incidentParms1, Map, Radius);
            PawnSpawnUtil.SpawnPawns(pawnList2, incidentParms2, Map, Radius);
            //设置集群AI
            ResolveLordJob(points, incidentParms1.spawnCenter, pawnList1, faction1, faction2);
            ResolveLordJob(points, incidentParms2.spawnCenter, pawnList2, faction2, faction1);
        }

        /// <summary>
        /// 创建集群AI
        /// </summary>
        /// <param name="points"></param>
        /// <param name="loc"></param>
        /// <param name="pawns"></param>
        /// <param name="assaultFaction"></param>
        /// <param name="targetFaction"></param>
        /// <returns></returns>
        private void ResolveLordJob(int points, IntVec3 loc, IEnumerable<Pawn> pawns, Faction assaultFaction,
            Faction targetFaction)
        {
            var siegeSpot = RCellFinder.FindSiegePositionFrom(loc, Map);
            //蓝图点数
            var blueprintPoints = points * Rand.Range(0.2f, 0.3f);
            if (blueprintPoints < MinBlueprintPoints)
            {
                blueprintPoints = MinBlueprintPoints;
            }

            var lordJobShellFactionFirst =
                new LordJobShellFactionFirst(assaultFaction, targetFaction, siegeSpot, blueprintPoints);
            LordMaker.MakeNewLord(assaultFaction, lordJobShellFactionFirst, Map, pawns);
        }
    }
}