// ******************************************************************
//       /\ /|       @file       SiteFactionWarContention.cs
//       \ V/        @brief      派系争夺战场地
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 09:14:47
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
    public class SiteFactionWarContention : Site
    {
        private const int FactionPoints = 10000;

        /// <summary>
        /// 生成地图后回调
        /// </summary>
        public override void PostMapGenerate()
        {
            //生成两个相互敌对的派系 设置集群AI互相攻击并争夺资源
            FactionUtil.GetHostileFactionPair(FactionPoints, PawnGroupKindDefOf.Combat, Find.FactionManager.AllFactionsVisible.ToList(),
                out var faction1, out var faction2);
            if (faction1 == null || faction2 == null)
            {
                return;
            }
            //创建派系1的角色 空投到地图中心
            var incidentParms1 = new IncidentParms {points = FactionPoints, faction = faction1, target = Map};
            var pawnGroupMakerParms1 =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms1);
            var pawnList1 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms1).ToList();
            var lordJob1 = new LordJobFactionContention(Map.Center);
            LordMaker.MakeNewLord(faction1, lordJob1, Map, pawnList1);
            DropPodUtility.DropThingsNear(Map.Center, Map, pawnList1);
            //创建派系2的角色 空投到地图中心
            var incidentParms2 = new IncidentParms {points = FactionPoints, faction = faction2, target = Map};
            var pawnGroupMakerParms2 =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms2);
            var pawnList2 = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms2).ToList();
            var lordJob2 = new LordJobFactionContention(Map.Center);
            LordMaker.MakeNewLord(faction2, lordJob2, Map, pawnList2);
            DropPodUtility.DropThingsNear(Map.Center, Map, pawnList2);
        }
    }
}
