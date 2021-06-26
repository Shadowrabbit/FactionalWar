// ******************************************************************
//       /\ /|       @file       SymbolResolverTempCamp.cs
//       \ V/        @brief      特征 临时营地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 18:06:26
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class SymbolResolverTempCamp : SymbolResolver
    {
        private static readonly FloatRange DefaultPawnsPoints = new FloatRange(5000f, 8000f);
        private const float WanderRadius = 35f;

        public override void Resolve(ResolveParams rp)
        {
            //全局缓存的地图
            var map = BaseGen.globalSettings.map;
            //解决方案中的派系 或者随机一个敌对派系
            var faction = rp.faction ??
                          Find.FactionManager.RandomEnemyFaction(false, false, false, TechLevel.Industrial);
            //边缘防卫宽度是否有值
            var dist = 0;
            if (rp.edgeDefenseWidth.HasValue)
            {
                dist = rp.edgeDefenseWidth.Value;
            }
            else if (rp.rect.Width >= 20 && rp.rect.Height >= 20 &&
                     (faction.def.techLevel >= TechLevel.Industrial || Rand.Bool))
            {
                dist = Rand.Bool ? 2 : 4;
            }

            var f = (float) (rp.rect.Area / 144.0 * 0.170000001788139);
            BaseGen.globalSettings.minEmptyNodes = (double) f < 1.0 ? 0 : GenMath.RoundRandom(f);
            //集群AI防卫
            var lordJobDefendPoint = new LordJob_DefendPoint(rp.rect.CenterCell, WanderRadius, false, false);
            var lord = rp.singlePawnLord ?? LordMaker.MakeNewLord(faction, lordJobDefendPoint, map);
            var lordToilDefendPoint = lord.Graph.FindToil<LordToil_DefendPoint>();
            if (!(lordToilDefendPoint.data is LordToilData_DefendPoint data))
            {
                Log.Error("can't find LordToilData_DefendPoint");
                return;
            }

            data.defendRadius = 35;
            var traverseParms = TraverseParms.For(TraverseMode.PassDoors);
            var resolveParams1 = rp;
            resolveParams1.rect = rp.rect;
            resolveParams1.faction = faction;
            resolveParams1.singlePawnLord = lord;
            resolveParams1.pawnGroupKindDef = rp.pawnGroupKindDef ?? PawnGroupKindDefOf.Combat;
            resolveParams1.singlePawnSpawnCellExtraPredicate = rp.singlePawnSpawnCellExtraPredicate ?? (x =>
                map.reachability.CanReachMapEdge(x, traverseParms));
            if (resolveParams1.pawnGroupMakerParams == null)
            {
                resolveParams1.pawnGroupMakerParams = new PawnGroupMakerParms
                {
                    tile = map.Tile,
                    faction = faction
                };
                var groupMakerParams = resolveParams1.pawnGroupMakerParams;
                groupMakerParams.points = DefaultPawnsPoints.RandomInRange;
                resolveParams1.pawnGroupMakerParams.inhabitants = true;
                resolveParams1.pawnGroupMakerParams.seed = rp.settlementPawnGroupSeed;
            }

            //生成角色
            BaseGen.symbolStack.Push("pawnGroup", resolveParams1);
            //生成灯
            BaseGen.symbolStack.Push("outdoorLighting", rp);
            //泡沫灭火器
            if (faction.def.techLevel >= TechLevel.Industrial)
            {
                var num = Rand.Chance(0.75f) ? GenMath.RoundRandom(rp.rect.Area / 400f) : 0;
                for (var index = 0; index < num; ++index)
                {
                    var resolveParams2 = rp;
                    resolveParams2.faction = faction;
                    BaseGen.symbolStack.Push("firefoamPopper", resolveParams2);
                }
            }

            bool? nullable1;
            if (dist > 0)
            {
                var resolveParams2 = rp;
                resolveParams2.faction = faction;
                resolveParams2.edgeDefenseWidth = dist;
                ref var local = ref resolveParams2;
                nullable1 = rp.edgeThingMustReachMapEdge;
                var nullable2 = new bool?(!nullable1.HasValue || nullable1.GetValueOrDefault());
                local.edgeThingMustReachMapEdge = nullable2;
                BaseGen.symbolStack.Push("edgeDefense", resolveParams2);
            }

            var resolveParams3 = rp;
            resolveParams3.rect = rp.rect.ContractedBy(dist);
            resolveParams3.faction = faction;
            BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParams3);
            var resolveParams4 = rp;
            resolveParams4.rect = rp.rect.ContractedBy(dist);
            resolveParams4.faction = faction;
            ref var local1 = ref resolveParams4;
            nullable1 = rp.floorOnlyIfTerrainSupports;
            var nullable3 = new bool?(!nullable1.HasValue || nullable1.GetValueOrDefault());
            local1.floorOnlyIfTerrainSupports = nullable3;
            BaseGen.symbolStack.Push("basePart_outdoors", resolveParams4);
            var resolveParams5 = rp;
            resolveParams5.floorDef = TerrainDefOf.Bridge;
            ref var local2 = ref resolveParams5;
            nullable1 = rp.floorOnlyIfTerrainSupports;
            var nullable4 = new bool?(!nullable1.HasValue || nullable1.GetValueOrDefault());
            local2.floorOnlyIfTerrainSupports = nullable4;
            ref var local3 = ref resolveParams5;
            nullable1 = rp.allowBridgeOnAnyImpassableTerrain;
            var nullable5 = new bool?(!nullable1.HasValue || nullable1.GetValueOrDefault());
            local3.allowBridgeOnAnyImpassableTerrain = nullable5;
            BaseGen.symbolStack.Push("floor", resolveParams5);
        }
    }
}