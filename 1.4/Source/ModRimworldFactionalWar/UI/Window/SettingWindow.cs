// ******************************************************************
//       /\ /|       @file       SettingWindow.cs
//       \ V/        @brief      模组设置页面
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-08-21 05:15:01
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class SettingWindow : Mod
    {
        public readonly SettingModel settingModel;
        public static SettingWindow Instance { get; private set; }

        public SettingWindow(ModContentPack content) : base(content)
        {
            settingModel = GetSettings<SettingModel>();
            Instance = this;
        }
        
        public override string SettingsCategory()
        {
            return "Factional War";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var ls = new Listing_Standard();
            ls.Begin(inRect);
            Text.Font = GameFont.Small;
            ls.CheckboxLabeled("SrNeedOptimization".Translate(), ref settingModel.needOptimization,
                "SrDescNeedOptimization".Translate());
            ls.GapLine(15f);
            ls.Label($"{"SrThreatPointFactor".Translate()}: {settingModel.threatPointFactor.ToStringPercent()}",
                tooltip: "SrDescThreatPointFactor".Translate());
            settingModel.threatPointFactor = ls.Slider(settingModel.threatPointFactor, 0.1f, 2f);
            ls.GapLine(15f);
            if (ls.ButtonText("Default")) settingModel.SetDefault();
            ls.End();
        }
    }
}