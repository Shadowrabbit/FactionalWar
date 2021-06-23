// ******************************************************************
//       /\ /|       @file       IncidentWorkerFactionWarRaid.cs
//       \ V/        @brief      派系战争袭击
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-22 12:05:02
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class IncidentWorkerFactionWarRaid : IncidentWorkerFactionWarShelling
    {
        private const int MinDist = 4; //生成在玩家最近几格
        private const int MaxDist = 7; //生成在玩家最远几格

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            return true;
        }
    }
}