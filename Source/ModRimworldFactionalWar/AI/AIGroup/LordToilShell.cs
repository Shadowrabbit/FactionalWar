// ******************************************************************
//       /\ /|       @file       LordToilShell.cs
//       \ V/        @brief      集群AI流程 炮击
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-26 19:55:17
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilShell : LordToil_Siege
    {
        private static readonly FloatRange BuilderCountFraction = new FloatRange(0.25f, 0.4f);

        public LordToilShell(IntVec3 siegeCenter, float blueprintPoints) : base(siegeCenter, blueprintPoints)
        {
        }

        public override void Init()
        {
            //设置半径
            var toilDataSiege = (LordToilData_Siege) data;
            toilDataSiege.baseRadius = Mathf.InverseLerp(14f, 25f, lord.ownedPawns.Count / 50f);
            toilDataSiege.baseRadius = Mathf.Clamp(toilDataSiege.baseRadius, 14f, 25f);
            //需要的建筑材料列表
            var source = new List<Thing>();
            //遍历蓝图
            foreach (var placeBlueprint in SiegeBlueprintPlacer.PlaceBlueprints(toilDataSiege.siegeCenter, Map,
                lord.faction, toilDataSiege.blueprintPoints))
            {
                toilDataSiege.blueprints.Add(placeBlueprint);
                //当前蓝图消耗的材料和数量
                foreach (var thingDefCountClass in placeBlueprint.MaterialsNeeded())
                {
                    var cost = thingDefCountClass;
                    //列表中包含当前材料
                    var thing1 = source.FirstOrDefault(t => t.def == cost.thingDef);
                    //添加数量
                    if (thing1 != null)
                    {
                        thing1.stackCount += cost.count;
                    }
                    //创建新的物品加入列表
                    else
                    {
                        var thing2 = ThingMaker.MakeThing(cost.thingDef);
                        thing2.stackCount = cost.count;
                        source.Add(thing2);
                    }
                }

                //如果蓝图是炮塔
                if (!(placeBlueprint.def.entityDefToBuild is ThingDef entityDefToBuild))
                {
                    continue;
                }

                //随机炮弹
                var randomShellDef = TurretGunUtility.TryFindRandomShellDef(entityDefToBuild, true,
                    techLevel: lord.faction.def.techLevel, maxMarketValue: 250f);
                if (randomShellDef == null)
                {
                    continue;
                }

                var thing = ThingMaker.MakeThing(randomShellDef);
                thing.stackCount = 5;
                source.Add(thing);
            }

            //数量修正 多带最多20%材料作为应急
            foreach (var t in source)
            {
                t.stackCount = Mathf.CeilToInt(t.stackCount * Rand.Range(1f, 1.2f));
            }

            //根据最终数量计算堆叠
            var thingsGroups = new List<List<Thing>>();
            for (var index = 0; index < source.Count; ++index)
            {
                while (source[index].stackCount > source[index].def.stackLimit)
                {
                    var num = Mathf.CeilToInt(source[index].def.stackLimit * Rand.Range(0.9f, 0.999f));
                    var thing = ThingMaker.MakeThing(source[index].def);
                    thing.stackCount = num;
                    source[index].stackCount -= num;
                    source.Add(thing);
                }
            }

            var thingList1 = new List<Thing>();
            for (var index = 0; index < source.Count; ++index)
            {
                thingList1.Add(source[index]);
                //奇数或者最后一项 分为一组
                if (index % 2 != 1 && index != source.Count - 1)
                {
                    continue;
                }

                thingsGroups.Add(thingList1);
                thingList1 = new List<Thing>();
            }

            var thingList2 = new List<Thing>();
            //添加到组
            thingsGroups.Add(thingList2);
            //发送空投
            DropPodUtility.DropThingGroupsNear(toilDataSiege.siegeCenter, Map, thingsGroups);
            var lordToilDataSiege = toilDataSiege;
            var randomInRange = BuilderCountFraction.RandomInRange;
            //设置建造分数需求
            lordToilDataSiege.desiredBuilderFraction = randomInRange;
        }
    }
}