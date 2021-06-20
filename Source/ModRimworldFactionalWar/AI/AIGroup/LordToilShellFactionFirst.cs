// ******************************************************************
//       /\ /|       @file       LordToilShellFactionFirst.cs
//       \ V/        @brief      集群AI流程 炮击派系优先
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-20 00:38:52
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordToilShellFactionFirst : LordToil_Siege
    {
        public LordToilShellFactionFirst(IntVec3 siegeCenter, float blueprintPoints) : base(siegeCenter,
            blueprintPoints)
        {
        }

        public override void Init()
        {
            base.Init();
        }
    }
}