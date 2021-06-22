// ******************************************************************
//       /\ /|       @file       SitePartDefOf.cs
//       \ V/        @brief      场地部分构成定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-22 12:24:07
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;

namespace SR.ModRimWorld.FactionalWar
{
    [DefOf]
    public static class SitePartDefOf
    {
        [UsedImplicitly] public static readonly SitePartDef SrOutpost; //处于战斗中的哨所
        [UsedImplicitly] public static readonly SitePartDef SrFactionWarShelling; //派系炮击战
    }
}
