// ******************************************************************
//       /\ /|       @file       IncidentWorkerFactionWar.cs
//       \ V/        @brief      事件 派系战争
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-10 23:56:29
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;

namespace SR.ModRimworld.FactionalWar
{
    public class IncidentWorkerFactionWar : IncidentWorker_PawnsArrive
    {
        /// <summary>
        /// 是否可以生成事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            var map = (Map) parms.target;
            //候选派系列表
            var candidateFactionList = CandidateFactions(map);
            //todo 需要存在两个互相敌对的派系
            return true;
        }
    }
}