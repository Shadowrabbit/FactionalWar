// ******************************************************************
//       /\ /|       @file       IncidentDefOf.cs
//       \ V/        @brief      事件定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-22 23:54:07
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;

namespace SR.ModRimWorld.FactionalWar
{
    [DefOf]
    public static class IncidentDefOf
    {
        [UsedImplicitly] public static readonly IncidentDef SrFactionWarShellingSiteGenerate; //派系炮击场地生成
        [UsedImplicitly] public static readonly IncidentDef SrFactionWarTempCampSiteGenerate; //派系临时营地生成
        [UsedImplicitly] public static readonly IncidentDef SrFactionWarContentionSiteGenerate; //派系争夺战场地生成
    }
}