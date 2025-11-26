using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace UniversalZoningSystem
{
    [SettingsUIGroupOrder(
        "NA_Residential",
        "EU_Residential",
        "NA_Commercial",
        "EU_Commercial",
        "Other"
    )]
    public class Setting : ModSetting
    {
        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NALowRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NARowRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NAMedRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NAHighRes { get; set; }

        [SettingsUISection("NA_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NAMixed { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EULowRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EURowRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EUMedRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EUHighRes { get; set; }

        [SettingsUISection("EU_Residential")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EUMixed { get; set; }

        [SettingsUISection("NA_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NAComLow { get; set; }

        [SettingsUISection("NA_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int NAComHigh { get; set; }

        [SettingsUISection("EU_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EUComLow { get; set; }

        [SettingsUISection("EU_Commercial")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int EUComHigh { get; set; }

        [SettingsUISection("Other")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int Office { get; set; }

        [SettingsUISection("Other")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int Industrial { get; set; }

        [SettingsUISection("Other")]
        [SettingsUISlider(min = 0, max = 100, step = 1)]
        public int Other { get; set; }

        public override void SetDefaults()
        {
            NALowRes = 100;
            NARowRes = 100;
            NAMedRes = 100;
            NAHighRes = 100;
            NAMixed = 100;

            EULowRes = 100;
            EURowRes = 100;
            EUMedRes = 100;
            EUHighRes = 100;
            EUMixed = 100;

            NAComLow = 100;
            NAComHigh = 100;
            EUComLow = 100;
            EUComHigh = 100;

            Office = 100;
            Industrial = 100;
            Other = 100;
        }
    }
}
