// ******************************************************************
//       /\ /|       @file       KidnapAIUtil.cs
//       \ V/        @brief      绑架AI工具
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-24 23:22:47
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    public static class KidnapAIUtil
    {
        public static bool TryFindGoodKidnapVictim(
            Pawn kidnapper,
            float maxDist,
            out Pawn victim,
            List<Thing> disallowed = null)
        {
            if (!kidnapper.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) ||
                !kidnapper.Map.reachability.CanReachMapEdge(kidnapper.Position,
                    TraverseParms.For(kidnapper, Danger.Some)))
            {
                victim = null;
                return false;
            }

            bool Validator(Thing t)
            {
                return t is Pawn pawn && pawn.RaceProps.Humanlike && pawn.Downed &&
                       pawn.Faction.HostileTo(kidnapper.Faction) && kidnapper.CanReserve((LocalTargetInfo) pawn) &&
                       (disallowed == null || !disallowed.Contains(pawn));
            }

            victim = (Pawn) GenClosest.ClosestThingReachable(kidnapper.Position, kidnapper.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell,
                TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), maxDist, Validator);
            return victim != null;
        }
    }
}