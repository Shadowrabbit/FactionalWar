// ******************************************************************
//       /\ /|       @file       RaidStrategyDefOf.cs
//       \ V/        @brief      袭击策略定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-12 20:32:02
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;

namespace SR.ModRimworld.FactionalWar
{
    [DefOf]
    public static class RaidStrategyDefOf
    {
        [UsedImplicitly] public static readonly RaidStrategyDef SrFactionFirst; //派系优先
    }
}