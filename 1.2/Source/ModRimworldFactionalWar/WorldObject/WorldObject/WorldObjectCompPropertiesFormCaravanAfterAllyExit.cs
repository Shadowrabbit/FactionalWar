// ******************************************************************
//       /\ /|       @file       WorldObjectCompPropertiesFormCaravanAfterAllyExit.cs
//       \ V/        @brief      世界物体组件属性 在友军离开后重组远征队
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-06 23:51:59
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class WorldObjectCompPropertiesFormCaravanAfterAllyExit : WorldObjectCompProperties_FormCaravan
    {
        public WorldObjectCompPropertiesFormCaravanAfterAllyExit() =>
            compClass = typeof(WorldCompFormCaravanAfterAllyExit);
    }
}
