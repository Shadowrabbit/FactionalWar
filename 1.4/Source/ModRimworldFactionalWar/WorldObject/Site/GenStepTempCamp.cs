// ******************************************************************
//       /\ /|       @file       GenStepTempCamp.cs
//       \ V/        @brief      生成步骤 临时营地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 17:36:51
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class GenStepTempCamp : GenStep
    {
        public override int SeedPart => 546950703;
        private const int Size = 70;
        private FloatRange _defaultPawnGroupPointsRange = new FloatRange(5000, 10000);

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        public override void Generate(Map map, GenStepParams parms)
        {
            var points = _defaultPawnGroupPointsRange.RandomInRange;
            //感兴趣的区域不存在
            if (!MapGenerator.TryGetVar("RectOfInterest", out CellRect cellCenter))
                //从中心取个矩形
                cellCenter = CellRect.SingleCell(map.Center);
            //使用中的区域
            if (!MapGenerator.TryGetVar("UsedRects", out List<CellRect> var2))
            {
                var2 = new List<CellRect>();
                MapGenerator.SetVar("UsedRects", var2);
            }

            //派系
            var faction = map.ParentFaction == null || map.ParentFaction == Faction.OfPlayer
                ? Find.FactionManager.RandomEnemyFaction()
                : map.ParentFaction;
            var resolveParams = new ResolveParams
            {
                rect = GetOutpostRect(cellCenter, var2),
                faction = faction,
                edgeDefenseWidth = 4,
                edgeDefenseTurretsCount = Rand.RangeInclusive(3, 6),
                edgeDefenseMortarsCount = Rand.RangeInclusive(1, 2),
                settlementPawnGroupPoints = points
            };
            //获取据点所在的矩形
            if (parms.sitePart != null)
            {
                resolveParams.settlementPawnGroupSeed =
                    OutpostSitePartUtility.GetPawnGroupMakerSeed(parms.sitePart.parms);
                parms.sitePart.parms.threatPoints = points;
            }

            BaseGen.globalSettings.map = map;
            BaseGen.globalSettings.minBuildings = 1;
            BaseGen.globalSettings.minBarracks = 1;
            BaseGen.symbolStack.Push("srTempCamp", resolveParams);
            if (faction != null && faction == Faction.OfEmpire)
            {
                BaseGen.globalSettings.minThroneRooms = 1;
                BaseGen.globalSettings.minLandingPads = 1;
            }

            BaseGen.Generate();
            if (faction != null && faction == Faction.OfEmpire && BaseGen.globalSettings.landingPadsGenerated == 0)
            {
                GenStep_Settlement.GenerateLandingPadNearby(resolveParams.rect, map, faction, out var usedRect);
                var2.Add(usedRect);
            }

            var2.Add(resolveParams.rect);
        }

        private static CellRect GetOutpostRect(
            CellRect rectToDefend,
            ICollection<CellRect> usedRects)
        {
            var cellRect = new CellRect(rectToDefend.minX - 1 - Size / 2, rectToDefend.minZ - 1 - Size / 2, Size, Size);
            usedRects.Add(cellRect);
            return cellRect;
        }
    }
}