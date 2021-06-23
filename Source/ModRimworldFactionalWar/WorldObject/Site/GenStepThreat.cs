// ******************************************************************
//       /\ /|       @file       GenStepThreat.cs
//       \ V/        @brief      生成步骤 威胁
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 17:59:43
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class GenStepThreat : GenStep
    {
        public override int SeedPart => 1254675168;
        private const int ThreatPoints = 5000;

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        public override void Generate(Map map, GenStepParams parms)
        {
            var incidentParms = new IncidentParms {points = ThreatPoints, faction = Faction.OfMechanoids, target = map};
            var pawnGroupMakerParms =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms);
            //生成失败 尝试用默认角色组生成器
            var pawnList = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            //设置集群AI
            foreach (var pawn in pawnList)
            {
                pawn.mindState.duty = new PawnDuty(RimWorld.DutyDefOf.Defend);
            }

            //生成机械族或者虫族
            foreach (var mechPawn in pawnList)
            {
                var loc = CellFinder.RandomClosewalkCellNear(map.Center, map, 5);
                GenSpawn.Spawn(mechPawn, loc, map);
            }
        }
    }
}