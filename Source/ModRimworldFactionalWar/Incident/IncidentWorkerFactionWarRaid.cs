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
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var temp = new List<SitePartDef>();
            temp.Add(SitePartDefOf.SrCampInWar);
            TileFinder.TryFindNewSiteTile(out var tileId, 4, 7);
            var site = SiteMaker.TryMakeSite(temp, tileId, false, null, true, 10000);
            Find.WorldObjects.Add(site);
            parms.target = site.Map;
            base.TryExecuteWorker(parms);
            return true;
        }
    }
}
