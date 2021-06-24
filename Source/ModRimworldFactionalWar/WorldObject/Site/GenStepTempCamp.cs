// ******************************************************************
//       /\ /|       @file       GenStepTempCamp.cs
//       \ V/        @brief      生成步骤 临时营地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 17:36:51
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using System.Collections.Generic;
using System.Linq;
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
        private const int Size = 80;
        private FloatRange _defaultPawnGroupPointsRange = new FloatRange(5000f, 8000f);
        private static readonly List<CellRect> PossibleRects = new List<CellRect>();
        public override void Generate(Map map, GenStepParams parms)
        {
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
                rect = GetOutpostRect(cellCenter, var2, map),
                faction = faction,
                edgeDefenseWidth = 4,
                edgeDefenseTurretsCount = Rand.RangeInclusive(4, 8),
                edgeDefenseMortarsCount = Rand.RangeInclusive(2, 4)
            };
            //获取据点所在的矩形
            if (parms.sitePart != null)
            {
                resolveParams.settlementPawnGroupPoints = parms.sitePart.parms.threatPoints;
                resolveParams.settlementPawnGroupSeed = OutpostSitePartUtility.GetPawnGroupMakerSeed(parms.sitePart.parms);
            }
            else
            {
                resolveParams.settlementPawnGroupPoints = _defaultPawnGroupPointsRange.RandomInRange;
            }
            BaseGen.globalSettings.map = map;
            BaseGen.globalSettings.minBuildings = 1;
            BaseGen.globalSettings.minBarracks = 1;
            BaseGen.symbolStack.Push("srTempCamp", resolveParams);
            if (faction != null && faction == Faction.Empire)
            {
                BaseGen.globalSettings.minThroneRooms = 1;
                BaseGen.globalSettings.minLandingPads = 1;
            }
            BaseGen.Generate();
            if (faction != null && faction == Faction.Empire && BaseGen.globalSettings.landingPadsGenerated == 0)
            {
                GenStep_Settlement.GenerateLandingPadNearby(resolveParams.rect, map, faction, out var usedRect);
                var2.Add(usedRect);
            }
            var2.Add(resolveParams.rect);
        }
        private static CellRect GetOutpostRect(
            CellRect rectToDefend,
            List<CellRect> usedRects,
            Map map)
        {
            //将四个方向的矩形加入可行列表
            PossibleRects.Add(new CellRect(rectToDefend.minX - 1 - Size, rectToDefend.CenterCell.z - Size / 2, Size, Size));
            PossibleRects.Add(new CellRect(rectToDefend.maxX + 1, rectToDefend.CenterCell.z - Size / 2, Size, Size));
            PossibleRects.Add(new CellRect(rectToDefend.CenterCell.x - Size / 2, rectToDefend.minZ - 1 - Size, Size, Size));
            PossibleRects.Add(new CellRect(rectToDefend.CenterCell.x - Size / 2, rectToDefend.maxZ + 1, Size, Size));
            var mapRect = new CellRect(0, 0, map.Size.x, map.Size.z);
            //撤销有覆盖的区域
            PossibleRects.RemoveAll(x => !x.FullyContainedWithin(mapRect));
            //没有剩余区域
            if (!PossibleRects.Any())
            {
                return rectToDefend;
            }
            //寻找未使用过的区域
            var source = PossibleRects.Where(x => !usedRects.Any(x.Overlaps));
            var enumerable = source as CellRect[] ?? source.ToArray();
            if (enumerable.Any())
            {
                return enumerable.Any() ? enumerable.RandomElement() : PossibleRects.RandomElement();
            }
            PossibleRects.Add(new CellRect(rectToDefend.minX - 1 - Size * 2, rectToDefend.CenterCell.z - Size / 2, Size, Size));
            PossibleRects.Add(new CellRect(rectToDefend.maxX + 1 + Size, rectToDefend.CenterCell.z - Size / 2, Size, Size));
            PossibleRects.Add(new CellRect(rectToDefend.CenterCell.x - Size / 2, rectToDefend.minZ - 1 - Size * 2, Size, Size));
            PossibleRects.Add(new CellRect(rectToDefend.CenterCell.x - Size / 2, rectToDefend.maxZ + 1 + Size, Size, Size));
            return enumerable.Any() ? enumerable.RandomElement() : PossibleRects.RandomElement();
        }
    }
}
