using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using System;

namespace UniversalZoningSystem
{
    [SettingsUIGroupOrder(
        "NA_Residential",
        "EU_Residential",
        "NA_Commercial",
        "EU_Commercial",
        "Other"
    )]
    [SettingsUIShowGroupName("NA_Residential", "EU_Residential", "NA_Commercial", "EU_Commercial", "Other")]
    public class Setting : ModSetting
    {
        // NOTE: These probability settings are currently not implemented in spawn logic.
        // They previously controlled prefab creation, which caused missing assets when changed.
        // TODO: Implement these as spawn weight modifiers in a future update
        // For now, they are kept for backwards compatibility with existing user settings.
        
        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NALowRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NARowRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NAMedRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NAHighRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NAMixed { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EULowRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EURowRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EUMedRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EUHighRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EUMixed { get; set; }

        [SettingsUISection("NA_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NAComLow { get; set; }

        [SettingsUISection("NA_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int NAComHigh { get; set; }

        [SettingsUISection("EU_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EUComLow { get; set; }

        [SettingsUISection("EU_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int EUComHigh { get; set; }

        [SettingsUISection("Other")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int Office { get; set; }

        [SettingsUISection("Other")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int Industrial { get; set; }

        [SettingsUISection("Other")]
        [SettingsUISlider(min = 0, max = 100, step = 10)]
        public int Other { get; set; }

        public override void SetDefaults()
        {
            NALowRes = 50;
            NARowRes = 50;
            NAMedRes = 50;
            NAHighRes = 50;
            NAMixed = 50;

            EULowRes = 50;
            EURowRes = 50;
            EUMedRes = 50;
            EUHighRes = 50;
            EUMixed = 50;

            NAComLow = 50;
            NAComHigh = 50;
            EUComLow = 50;
            EUComHigh = 50;

            Office = 50;
            Industrial = 50;
            Other = 50;
        }
    }
}
