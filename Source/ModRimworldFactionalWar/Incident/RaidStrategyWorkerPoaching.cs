// ******************************************************************
//       /\ /|       @file       RaidStrategyWorkerPoaching.cs
//       \ V/        @brief      袭击策略 偷猎
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:01:36
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
    public class RaidStrategyWorkerPoaching : RaidStrategyWorker
    {
        private const int MaxPawnCount = 7; //最大生成角色数量
        private const int MinPawnCount = 1; //最小生成角色数量

        /// <summary>
        /// 创建集群AI工作
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="map"></param>
        /// <param name="pawns"></param>
        /// <param name="raidSeed"></param>
        /// <returns></returns>
        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        {
            return new LordJobPoaching();
        }

        /// <summary>
        /// 生成角色
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public override List<Pawn> SpawnThreats(IncidentParms parms)
        {
            var pawnCount = new IntRange(MinPawnCount, MaxPawnCount).RandomInRange;
            var pawnGroupMakerParms =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);
            var pawnList = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            if (pawnList.Count == 0)
            {
                Log.Error($"Got no pawns spawning raid from parms {parms}");
                return pawnList;
            }

            //删掉多余的数量
            if (pawnList.Count > pawnCount)
            {
                for (var i = pawnList.Count - 1; i > pawnCount; i--)
                {
                    pawnList.RemoveAt(i);
                }
            }
            parms.raidArrivalMode.Worker.Arrive(pawnList, parms);
            return pawnList;
        }
    }
}
