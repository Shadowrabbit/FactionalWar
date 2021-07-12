// ******************************************************************
//       /\ /|       @file       StateGraphExtention.cs
//       \ V/        @brief      集群AI状态机扩展
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 09:40:23
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public static class StateGraphExtention
    {
        public static T FindToil<T>(this StateGraph stateGraph)
        {
            foreach (var lordToil in stateGraph.lordToils)
            {
                if (lordToil is T targetToil)
                {
                    return targetToil;
                }
            }
            Log.Error($"[SR.ModRimWorld.FactionalWar]cant't find type {typeof(T)}");
            return default;
        }
    }
}
