﻿// ******************************************************************
//       /\ /|       @file       GenStepAirdropResource.cs
//       \ V/        @brief      生成步骤 空投资源
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 17:50:07
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class GenStepAirdropResource : GenStep
    {
        public override int SeedPart => 546950704;
        private const int ThreatPoints = 5000;

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        public override void Generate(Map map, GenStepParams parms)
        {
            //生成珍贵资源
            var thingSetMakerParams = default(ThingSetMakerParams);
            thingSetMakerParams.qualityGenerator = QualityGenerator.Reward;
            thingSetMakerParams.makingFaction = Find.FactionManager.AllFactionsListForReading.RandomElement();
            var totalThingList = new List<Thing>();
            for (var i = 0; i < 5; i++)
            {
                var betrayalRewardThings =
                    ThingSetMakerDefOf.MapGen_AncientTempleContents.root.Generate(thingSetMakerParams);
                totalThingList.AddRange(betrayalRewardThings);
            }

            //空投到中心
            DropPodUtility.DropThingsNear(map.Center, map, totalThingList);
            var incidentParms = new IncidentParms {points = ThreatPoints, faction = Faction.OfMechanoids, target = map};
            var pawnGroupMakerParms =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms);
            //生成失败 尝试用默认角色组生成器
            var pawnList = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            //空投到中心
            DropPodUtility.DropThingsNear(map.Center, map, pawnList);
            var lordJob =
                new LordJob_MechanoidsDefend(totalThingList, Faction.OfMechanoids, 5f, map.Center, false, false);
            LordMaker.MakeNewLord(Faction.OfMechanoids, lordJob, map, pawnList);
        }
    }
}