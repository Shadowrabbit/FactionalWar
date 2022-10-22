// ******************************************************************
//       /\ /|       @file       IncidentWorkerFactionWarShellingSiteGenerate.cs
//       \ V/        @brief      事件处理 派系炮击战 场地生成
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 09:52:28
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
    public class IncidentWorkerFactionWarShellingSiteGenerate : IncidentWorker
    {
        private const int MinDist = 2; //生成在玩家最近几格
        private const int MaxDist = 7; //生成在玩家最远几格

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var sitePartList = new List<SitePartDef> {SitePartDefOf.SrFactionWarShelling};
            //找世界坐标对应的id
            TileFinder.TryFindNewSiteTile(out var tileId, MinDist, MaxDist);
            //生成默认部分场地参数
            SiteMakerHelper.GenerateDefaultParams(parms.points, tileId, null, sitePartList,
                out var sitePartDefsWithParams);
            //场地
            var siteFactionWarShelling = (SiteFactionWarShelling)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf
                .SrSiteFactionWarShelling);
            siteFactionWarShelling.Tile = tileId;
            siteFactionWarShelling.factionMustRemainHostile = false;
            if (sitePartDefsWithParams != null)
            {
                foreach (var sitePart in sitePartDefsWithParams)
                    siteFactionWarShelling.AddPart(new SitePart(siteFactionWarShelling, sitePart.def, sitePart.parms));
            }

            siteFactionWarShelling.desiredThreatPoints = siteFactionWarShelling.ActualThreatPoints;
            Find.WorldObjects.Add(siteFactionWarShelling);
            //中立信件通知
            var letter = LetterMaker.MakeLetter("SrLabelFactionWarShelling".Translate(),
                "SrDescFactionWarShelling".Translate(),
                parms.customLetterDef ?? LetterDefOf.NeutralEvent,
                siteFactionWarShelling,
                parms.faction, parms.quest, parms.letterHyperlinkThingDefs);
            Find.LetterStack.ReceiveLetter(letter);
            return true;
        }
    }
}
