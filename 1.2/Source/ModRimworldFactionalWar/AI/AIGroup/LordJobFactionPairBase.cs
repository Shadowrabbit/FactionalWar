// ******************************************************************
//       /\ /|       @file       LordJobFactionPairBase.cs
//       \ V/        @brief      集群AI 派系对基础
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-20 15:00:04
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobFactionPairBase : LordJob
    {
        protected Faction faction; //派系
        protected Faction targetFaction; //敌对派系
        protected IntVec3 stageLoc; //集结中心
        public Faction TargetFaction => targetFaction;

        protected LordJobFactionPairBase()
        {
        }

        protected LordJobFactionPairBase(Faction faction, Faction targetFaction, IntVec3 stageLoc)
        {
            this.faction = faction;
            this.targetFaction = targetFaction;
            this.stageLoc = stageLoc;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_References.Look(ref targetFaction, "targetFaction");
            Scribe_Values.Look(ref stageLoc, "stageLoc");
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            return null;
        }
    }
}