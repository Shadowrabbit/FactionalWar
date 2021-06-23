// ******************************************************************
//       /\ /|       @file       SiteFactionWarShelling.cs
//       \ V/        @brief      派系炮击战地点
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-22 22:20:50
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class SiteFactionWarShelling : Site
    {
        /// <summary>
        /// 生成地图后回调
        /// </summary>
        public override void PostMapGenerate()
        {
            //根据派系炮击战事件生成角色
            var incidentParms = new IncidentParms {target = Map, points = desiredThreatPoints};
            IncidentDefOf.SrFactionWarShelling.Worker.TryExecute(incidentParms);
        }
    }
}