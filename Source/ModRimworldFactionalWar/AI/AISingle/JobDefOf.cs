// ******************************************************************
//       /\ /|       @file       JobDefOf.cs
//       \ V/        @brief      工作定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-16 14:27:56
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [DefOf]
    public static class JobDefOf
    {
        [UsedImplicitly] public static readonly JobDef SrKillMelee; //近战杀死敌人
        public static JobDef Goto { get; set; }
    }
}
