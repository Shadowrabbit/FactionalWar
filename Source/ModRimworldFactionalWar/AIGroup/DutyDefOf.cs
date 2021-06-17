// ******************************************************************
//       /\ /|       @file       DutyDefOf.cs
//       \ V/        @brief      责任定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 20:54:23
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse.AI;

namespace SR.ModRimWorld.FactionalWar
{
    [DefOf]
    public static class DutyDefOf
    {
        [UsedImplicitly] public static readonly DutyDef SrAssaultFactionFirst; //派系优先
        [UsedImplicitly] public static readonly DutyDef SrKillHostileFactionMember; //派系胜利 自我防卫 包扎伤口 击杀敌对派系成员
        [UsedImplicitly] public static readonly DutyDef SrClearBattlefield; // 清理战场物资
        [UsedImplicitly] public static readonly DutyDef SrLogging; // 伐木
        [UsedImplicitly] public static readonly DutyDef SrTakeWoodExit; // 带着木材离开
        [UsedImplicitly] public static readonly DutyDef SrPoaching; // 偷猎
        [UsedImplicitly] public static readonly DutyDef SrTakePreyExit; // 带着猎物离开
        [UsedImplicitly] public static readonly DutyDef SrShellingFactionsPriority; // 炮击派系优先
    }
}