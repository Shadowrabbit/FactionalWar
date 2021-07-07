// ******************************************************************
//       /\ /|       @file       WorldCompFormCaravanAfterAllyExit.cs
//       \ V/        @brief      重组远征队 在友军离开之后
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-06 23:47:29
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class WorldCompFormCaravanAfterAllyExit : FormCaravanComp
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            FormCaravanComp formCaravanComp = this;
            var mapParent = (MapParent) formCaravanComp.parent;
            if (!mapParent.HasMap)
            {
                yield break;
            }

            if (!formCaravanComp.Reform)
            {
                var commandAction = new Command_Action
                {
                    defaultLabel = "CommandFormCaravan".Translate(),
                    defaultDesc = "CommandFormCaravanDesc".Translate(),
                    icon = FormCaravanCommand,
                    hotKey = KeyBindingDefOf.Misc2,
                    tutorTag = "FormCaravan",
                    action = () =>
                        Find.WindowStack.Add(new Dialog_FormCaravan(mapParent.Map))
                };
                yield return commandAction;
            }

            if (mapParent.Map.mapPawns.FreeColonistsSpawnedCount != 0)
            {
                var commandAction = new Command_Action
                {
                    defaultLabel = "CommandReformCaravan".Translate(),
                    defaultDesc = "CommandReformCaravanDesc".Translate(),
                    icon = FormCaravanCommand,
                    hotKey = KeyBindingDefOf.Misc2,
                    tutorTag = "ReformCaravan",
                    action = () =>
                        Find.WindowStack.Add(new Dialog_FormCaravan(mapParent.Map, true))
                };
                if (!mapParent.Map.IsAllyExit())
                {
                    commandAction.Disable("SrCommandReformCaravanFailAlliesCollectingLoot".Translate());
                }

                if (GenHostility.AnyHostileActiveThreatToPlayer(mapParent.Map, true)
                )
                {
                    commandAction.Disable("CommandReformCaravanFailHostilePawns".Translate());
                }


                yield return commandAction;
            }

            if (!Prefs.DevMode)
            {
                yield break;
            }

            var commandActionDev = new Command_Action
            {
                defaultLabel = "Dev: Show available exits",
                action = () =>
                {
                    foreach (var tile in CaravanExitMapUtility.AvailableExitTilesAt(mapParent.Map))
                        Find.WorldDebugDrawer.FlashTile(tile, duration: 10);
                }
            };
            yield return commandActionDev;
        }
    }
}