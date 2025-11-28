using System.Collections.Generic;
using Colossal;

namespace UniversalZoningSystem
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Universal Zoning System" },
                { m_Setting.GetOptionTabLocaleID("NA_Residential"), "NA Residential" },
                { m_Setting.GetOptionTabLocaleID("EU_Residential"), "EU Residential" },
                { m_Setting.GetOptionTabLocaleID("NA_Commercial"), "NA Commercial" },
                { m_Setting.GetOptionTabLocaleID("EU_Commercial"), "EU Commercial" },
                { m_Setting.GetOptionTabLocaleID("Other"), "Other" },

                { m_Setting.GetOptionGroupLocaleID("NA_Residential"), "REQUIRES GAME RESTART - North American Residential building spawn chances. Settings only apply to newly placed zones after restarting the game." },
                { m_Setting.GetOptionGroupLocaleID("EU_Residential"), "REQUIRES GAME RESTART - European Residential building spawn chances. Settings only apply to newly placed zones after restarting the game." },
                { m_Setting.GetOptionGroupLocaleID("NA_Commercial"), "REQUIRES GAME RESTART - North American Commercial building spawn chances. Settings only apply to newly placed zones after restarting the game." },
                { m_Setting.GetOptionGroupLocaleID("EU_Commercial"), "REQUIRES GAME RESTART - European Commercial building spawn chances. Settings only apply to newly placed zones after restarting the game." },
                { m_Setting.GetOptionGroupLocaleID("Other"), "REQUIRES GAME RESTART - Other building spawn chances. Settings only apply to newly placed zones after restarting the game." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NALowRes)), "NA Low Density Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NALowRes)), "Probability of North American Low Density buildings (0-100: percentage chance, 100: always include)." },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NARowRes)), "NA Row Housing Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NARowRes)), "Probability of North American Row Housing buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NAMedRes)), "NA Medium Density Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NAMedRes)), "Probability of North American Medium Density buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NAHighRes)), "NA High Density Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NAHighRes)), "Probability of North American High Density buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NAMixed)), "NA Mixed Housing Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NAMixed)), "Probability of North American Mixed Housing buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EULowRes)), "EU Low Density Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EULowRes)), "Probability of European Low Density buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EURowRes)), "EU Row Housing Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EURowRes)), "Probability of European Row Housing buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EUMedRes)), "EU Medium Density Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EUMedRes)), "Probability of European Medium Density buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EUHighRes)), "EU High Density Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EUHighRes)), "Probability of European High Density buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EUMixed)), "EU Mixed Housing Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EUMixed)), "Probability of European Mixed Housing buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NAComLow)), "NA Low Commercial Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NAComLow)), "Probability of North American Low Commercial buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NAComHigh)), "NA High Commercial Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NAComHigh)), "Probability of North American High Commercial buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EUComLow)), "EU Low Commercial Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EUComLow)), "Probability of European Low Commercial buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EUComHigh)), "EU High Commercial Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EUComHigh)), "Probability of European High Commercial buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Office)), "Office Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Office)), "Probability of Office buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Industrial)), "Industrial Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Industrial)), "Probability of Industrial buildings (0-100: percentage chance, 100: always include)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Other)), "Other / Region Packs Chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Other)), "Probability of buildings from other Region Packs (0-100: percentage chance, 100: always include)." },

                // Zone Names & Descriptions
                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ResLow + "]", "Universal Low Density Residential" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ResLow + "]", "Low density residential zone that allows both North American and European styles to spawn." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ResRow + "]", "Universal Row Housing" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ResRow + "]", "Medium density row housing that allows both North American and European styles to spawn." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ResMed + "]", "Universal Medium Density Residential" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ResMed + "]", "Medium density residential zone that allows both North American and European styles to spawn." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ResHigh + "]", "Universal High Density Residential" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ResHigh + "]", "High density residential zone that allows both North American and European styles to spawn." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ResLowRent + "]", "Universal Low Rent Housing" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ResLowRent + "]", "Large apartment buildings with small apartments and low rent. Allows mixed styles." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_Mixed + "]", "Universal Mixed Housing" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_Mixed + "]", "Mixed use zoning with commercial on the ground floor and residential above. Allows mixed styles." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ComLow + "]", "Universal Low Density Commercial" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ComLow + "]", "Low density commercial zone that allows both North American and European styles to spawn." },

                { "Assets.NAME[" + UniversalZoneDefinitions.Universal_ComHigh + "]", "Universal High Density Commercial" },
                { "Assets.DESCRIPTION[" + UniversalZoneDefinitions.Universal_ComHigh + "]", "High density commercial zone that allows both North American and European styles to spawn." },

                // UI Elements
                { "UniversalZoning.THEME_BUTTON_TITLE", "Universal Pack" },
                { "UniversalZoning.THEME_BUTTON_DESCRIPTION", "Click to show only Universal zones" },
                { "UniversalZoning.THEME_ACTIVE", "Universal theme active" },
                { "UniversalZoning.THEME_INACTIVE", "Universal theme inactive" },
            };
        }
        public void Unload()
        {
        }
    }
}
