// ******************************************************************
//       /\ /|       @file       IncidentWorkerFactionWar.cs
//       \ V/        @brief      事件 派系战争
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-10 23:56:29
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    public class IncidentWorkerFactionWar : IncidentWorker_Raid
    {
        private Faction _faction1;
        private Faction _faction2;
        private const int MaxRaidPoints = 5000; //最大袭击点数

        /// <summary>
        /// 是否可以生成事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error("[SR.ModRimWorld.FactionalWar]target must be a map.");
                return false;
            }

            //候选派系列表
            var candidateFactionList = CandidateFactions(map).ToList();
            //需要存在两个互相敌对的派系
            return (from faction in candidateFactionList
                from anotherFaction in candidateFactionList
                where faction.HostileTo(anotherFaction)
                select faction).Any();
        }

        /// <summary>
        /// 派系能否成为资源组
        /// </summary>
        /// <param name="f">派系</param>
        /// <param name="map">地图</param>
        /// <param name="desperate">绝望难度</param>
        /// <returns></returns>
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return base.FactionCanBeGroupSource(f, map, desperate) &&
                   (desperate || GenDate.DaysPassed >= f.def.earliestRaidDays);
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            //处理袭击点数
            ResolveRaidPoints(parms);
            //处理袭击派系
            if (!TryResolveRaidFactions(parms, out _faction1, out _faction2))
            {
                Log.Warning("[SR.ModRimWorld.FactionalWar]cant find raid factions");
                return false;
            }

            //设置事件参数1派系为第一个派系
            parms.faction = _faction1;
            //创建事件参数2给第二个派系
            var parms2 = new IncidentParms {faction = _faction2, points = parms.points, target = parms.target};
            //角色组定义 战斗
            var combat = PawnGroupKindDefOf.Combat;
            //解决袭击策略
            ResolveRaidStrategy(parms, combat);
            ResolveRaidStrategy(parms2, combat);
            //解决到达方式
            ResolveRaidArriveMode(parms);
            ResolveRaidArriveMode(parms2);
            //尝试生成威胁（参数）
            parms.raidStrategy.Worker.TryGenerateThreats(parms);
            parms2.raidStrategy.Worker.TryGenerateThreats(parms2);
            //尝试解决袭击召唤中心
            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                Log.Warning($"[SR.ModRimWorld.FactionalWar]cant resolve raid spawn center: {parms}");
                return false;
            }

            if (!parms2.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms2))
            {
                Log.Warning($"[SR.ModRimWorld.FactionalWar]cant resolve raid spawn center: {parms2}");
                return false;
            }

            //战利品生成点数
            var raidLootPoints = parms.points / 10;
            //根据策略再次调整袭击点数
            parms.points = AdjustedRaidPoints(parms.points, parms.raidArrivalMode,
                parms.raidStrategy, parms.faction, combat);
            parms2.points = parms.points;
            //生成派系部队
            var pawnListFaction1 = parms.raidStrategy.Worker.SpawnThreats(parms);
            ResolvePawnList(ref pawnListFaction1, parms);
            var pawnListFaction2 = parms2.raidStrategy.Worker.SpawnThreats(parms2);
            ResolvePawnList(ref pawnListFaction2, parms2);
            //设置角色携带战利品
            GenerateRaidLoot(parms, raidLootPoints, pawnListFaction1);
            GenerateRaidLoot(parms, raidLootPoints, pawnListFaction2);
            //信件通知
            SendLetter(parms, pawnListFaction1, parms2, pawnListFaction2);
            //根据分组生成集群AI
            if (!(parms.raidStrategy.Worker is RaidStrategyWorkerFactionFirst raidStrategyWorkerFactionFirst))
            {
                Log.Error("[SR.ModRimWorld.FactionalWar]strategy must be RaidStrategyWorkerFactionFirst");
                return false;
            }

            //互相设置敌对派系
            raidStrategyWorkerFactionFirst.TempTargetFaction = _faction2;
            raidStrategyWorkerFactionFirst.MakeLords(parms, pawnListFaction1);
            raidStrategyWorkerFactionFirst.TempTargetFaction = _faction1;
            raidStrategyWorkerFactionFirst.MakeLords(parms2, pawnListFaction2);
            //袭击时设置一倍速
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            //更新参与袭击的敌人记录
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;
            return true;
        }

        /// <summary>
        /// 原接口 不会被调用
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            return false;
        }

        /// <summary>
        /// 获取信件标签
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override string GetLetterLabel(IncidentParms parms)
        {
            return $"{parms.raidStrategy.letterLabelEnemy}: {_faction1?.Name}, {_faction2?.Name}";
        }

        /// <summary>
        /// 信件内容(派系领导人)
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="pawns"></param>
        /// <returns></returns>
        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            var pawn = pawns.Find(x => x.Faction.leader == x);
            if (pawn != null)
            {
                return "EnemyRaidLeaderPresent".Translate(pawn.Faction.def.pawnsPlural, pawn.LabelShort,
                    pawn.Named("LEADER"));
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取信件定义
        /// </summary>
        /// <returns></returns>
        protected override LetterDef GetLetterDef()
        {
            return LetterDefOf.ThreatBig;
        }

        /// <summary>
        /// 关系信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
        {
            return "LetterRelatedPawnsRaidEnemy".Translate(Faction.OfPlayer.def.pawnsPlural,
                $"{_faction1.def.pawnsPlural},{_faction2.def.pawnsPlural}");
        }

        /// <summary>
        /// 解决袭击点数
        /// </summary>
        /// <param name="parms"></param>
        protected override void ResolveRaidPoints(IncidentParms parms)
        {
            if (parms.points > MaxRaidPoints)
            {
                parms.points = MaxRaidPoints;
                return;
            }

            if (!(parms.points <= 0f))
            {
                return;
            }

            parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
        }

        /// <summary>
        /// 尝试解决突袭派系
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="faction1"></param>
        /// <param name="faction2"></param>
        /// <returns></returns>
        private bool TryResolveRaidFactions(IncidentParms parms, out Faction faction1,
            out Faction faction2)
        {
            FindFactionsInWar(parms, out faction1, out faction2);
            return faction1 != null && faction2 != null;
        }

        /// <summary>
        /// 解决突袭策略
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="groupKind"></param>
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            //派系优先
            parms.raidStrategy = RaidStrategyDefOf.SrFactionFirst;
        }

        /// <summary>
        /// 发送信件
        /// </summary>
        /// <param name="parms1"></param>
        /// <param name="pawns1"></param>
        /// <param name="parms2"></param>
        /// <param name="pawns2"></param>
        private void SendLetter(IncidentParms parms1, List<Pawn> pawns1, IncidentParms parms2, List<Pawn> pawns2)
        {
            TaggedString baseLetterLabel = GetLetterLabel(parms1);
            TaggedString baseLetterText = GetLetterText(parms1, pawns1, parms2, pawns2);
            var allfactionPawnList = new List<Pawn>();
            allfactionPawnList.AddRange(pawns1);
            allfactionPawnList.AddRange(pawns2);
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(allfactionPawnList, ref baseLetterLabel,
                ref baseLetterText, GetRelatedPawnsInfoLetterText(parms1), true);
            SendStandardLetter(baseLetterLabel, baseLetterText, GetLetterDef(), parms1, allfactionPawnList,
                Array.Empty<NamedArgument>());
        }

        /// <summary>
        /// 获取信件内容
        /// </summary>
        /// <param name="parms1"></param>
        /// <param name="pawns1"></param>
        /// <param name="parms2"></param>
        /// <param name="pawns2"></param>
        /// <returns></returns>
        private string GetLetterText(IncidentParms parms1, List<Pawn> pawns1, IncidentParms parms2, List<Pawn> pawns2)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(parms1.raidArrivalMode.textEnemy, _faction1.def.pawnsPlural,
                _faction1.Name.ApplyTag(parms1.faction), _faction2.def.pawnsPlural,
                _faction2.Name.ApplyTag(parms2.faction)).CapitalizeFirst());
            sb.Append("\n\n");
            sb.Append(string.Format(parms1.raidStrategy.arrivalTextEnemy, _faction1.def.pawnsPlural,
                _faction1.Name.ApplyTag(parms1.faction), _faction2.def.pawnsPlural,
                _faction2.Name.ApplyTag(parms2.faction)));
            sb.Append(GetLetterText(parms1, pawns1));
            sb.Append("\n");
            sb.Append(GetLetterText(parms2, pawns2));
            return sb.ToString();
        }

        /// <summary>
        /// 寻找处于交战中的两个势力
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="faction1"></param>
        /// <param name="faction2"></param>
        private void FindFactionsInWar(IncidentParms parms, out Faction faction1,
            out Faction faction2)
        {
            faction1 = null;
            faction2 = null;
            if (!(parms.target is Map map))
            {
                Log.Error("[SR.ModRimWorld.FactionalWar]target must be a map");
                return;
            }

            var points = parms.points; //袭击点数
            //全部派系
            var candidateFactionList = CandidateFactions(map).ToList();
            FactionUtil.GetHostileFactionPair(out faction1, out faction2, points, PawnGroupKindDefOf.Combat,
                candidateFactionList);
        }

        /// <summary>
        /// 校验角色列表
        /// </summary>
        /// <param name="pawnList"></param>
        /// <param name="parms"></param>
        private static void ResolvePawnList(ref List<Pawn> pawnList, IncidentParms parms)
        {
            if (pawnList != null)
            {
                return;
            }

            var pawnGroupMakerParms =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);
            //生成失败 尝试用默认角色组生成器
            pawnList = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
            if (pawnList.Count == 0)
            {
                Log.Error($"[SR.ModRimWorld.FactionalWar]Got no pawns spawning raid from parms {parms}");
                return;
            }

            parms.raidArrivalMode.Worker.Arrive(pawnList, parms);
        }
    }
}