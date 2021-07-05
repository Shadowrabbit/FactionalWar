// ******************************************************************
//       /\ /|       @file       PawnGroupMakerUtility.cs
//       \ V/        @brief      角色生成器
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-02 11:05:15
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class PawnGroupMakerUtility : BaseSingleTon<PawnGroupMakerUtility>
    {
        /// <summary>
        /// 生成角色
        /// </summary>
        /// <param name="pawnGroupMakerParms"></param>
        /// <returns></returns>
        public static List<Pawn> GeneratePawns(PawnGroupMakerParms pawnGroupMakerParms)
        {
            DiscardPawns();
            var pawnList = RimWorld.PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            return pawnList;
        }

        /// <summary>
        /// 拉黑角色
        /// </summary>
        private static void DiscardPawns()
        {
            var worldPawns = Find.World.worldPawns;
            DiscardPawns(worldPawns.GetPawnsBySituation(WorldPawnSituation.Free).ToList());
            DiscardPawns(worldPawns.GetPawnsBySituation(WorldPawnSituation.Dead).ToList());
            DiscardPawns(worldPawns.GetPawnsBySituation(WorldPawnSituation.Kidnapped).ToList());
        }

        /// <summary>
        /// 从世界缓存中拉黑角色列表
        /// </summary>
        /// <param name="pawnList"></param>
        private static void DiscardPawns(IReadOnlyList<Pawn> pawnList)
        {
            var worldPawns = Find.World.worldPawns;
            for (var i = pawnList.Count - 1; i > 0; i--)
            {
                //当前角色还没成为worldPawn
                if (!worldPawns.Contains(pawnList[i]))
                {
                    continue;
                }
                
                //玩家的角色
                if (pawnList[i].Faction != null && pawnList[i].Faction == Faction.OfPlayer)
                {
                    continue;
                }
                worldPawns.RemoveAndDiscardPawnViaGC(pawnList[i]);
            }
        }
    }
}
