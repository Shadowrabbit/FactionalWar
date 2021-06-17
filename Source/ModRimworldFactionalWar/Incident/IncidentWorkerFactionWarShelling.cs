// ******************************************************************
//       /\ /|       @file       IncidentWorkerFactionWarShelling.cs
//       \ V/        @brief      事件 派系炮击战
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:44:31
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class IncidentWorkerFactionWarShelling : IncidentWorker_Raid
    {
        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            throw new System.NotImplementedException();
        }
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            throw new System.NotImplementedException();
        }
        protected override string GetLetterLabel(IncidentParms parms)
        {
            throw new System.NotImplementedException();
        }
        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            throw new System.NotImplementedException();
        }
        protected override LetterDef GetLetterDef()
        {
            throw new System.NotImplementedException();
        }
        protected override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
        {
            throw new System.NotImplementedException();
        }
        protected override void ResolveRaidPoints(IncidentParms parms)
        {
            throw new System.NotImplementedException();
        }
    }
}
