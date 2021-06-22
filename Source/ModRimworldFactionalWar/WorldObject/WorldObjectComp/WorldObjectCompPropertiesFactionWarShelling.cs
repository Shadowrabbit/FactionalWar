// ******************************************************************
//       /\ /|       @file       WorldObjectCompPropertiesFactionWarShelling.cs
//       \ V/        @brief      世界组件参数 派系炮击战
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-22 22:57:07
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class WorldObjectCompPropertiesFactionWarShelling : WorldObjectCompProperties
    {
        public WorldObjectCompPropertiesFactionWarShelling()
        {
            compClass = typeof(WorldCompFactionWarShelling);
        }
    }
}