# Universal Zoning System - Migration Summary

## What was done

Successfully copied the Universal Zoning System (UZS) mod from the GitHub repository (https://github.com/redlabracer/Universal-Zoning-System--UZS-) to your local project for PDX Mods publishing.

**NEW:** Added a Universal Theme Button feature that allows players to filter and show only Universal zones in the zoning toolbar!

## Files Added/Updated

### New Files Added:
1. **LocaleEN.cs** - Localization strings for English
2. **UniversalZoneDefinitions.cs** - Constants defining the universal zone types
3. **Systems/ZoneMergeSystem.cs** - Core system that merges buildings into universal zones
4. **Systems/UniversalThemeUISystem.cs** - NEW! UI system for the Universal theme button
5. **ui_src/universal-theme-button.js** - NEW! JavaScript UI extension that adds the button to the toolbar
6. **mod.json** - Mod metadata file
7. **README.md** - Project documentation

### Updated Files:
1. **Mod.cs** - Replaced template code with actual UZS mod implementation + registered UI system
2. **Setting.cs** - Replaced template settings with UZS-specific settings (probability sliders for different building types)
3. **Properties/PublishConfiguration.xml** - Updated with proper mod information for PDX Mods publishing
4. **Universal Zoning System (UZS).csproj** - Added UI script files to build output

## Key Features of the Mod

### What it does:
- Creates "Universal" zones that accept both North American and European building styles
- Eliminates the need to constantly switch themes or paint districts
- Provides customizable spawn probabilities for different building types
- **NEW: Universal Theme Button** - Adds a purple "UP" (Universal Pack) button to the zoning toolbar that filters to show only Universal zones when clicked

### Available Universal Zones:
- Universal Low Density Residential
- Universal Row Housing
- Universal Medium Density Residential
- Universal High Density Residential
- Universal Low Rent Housing
- Universal Mixed Housing
- Universal Low Density Commercial
- Universal High Density Commercial

### Settings:
Users can adjust spawn probabilities (0-100%) for:
- NA Residential (Low, Row, Medium, High, Mixed)
- EU Residential (Low, Row, Medium, High, Mixed)
- NA Commercial (Low, High)
- EU Commercial (Low, High)
- Office buildings
- Industrial buildings
- Other region pack buildings

## NEW FEATURE: Universal Theme Button

### What it looks like:
- Purple gradient button with "UP" text (Universal Pack)
- Located in the theme section of the zoning toolbar
- Hover effect with scale animation and glow
- Active state shows darker purple when filtering is enabled

### How it works:
1. Player clicks the "UP" button in the zoning toolbar
2. All non-Universal zones are hidden/dimmed
3. Only Universal zones remain visible and selectable
4. Click again to show all zones

### Technical Implementation:
- **C# System**: `UniversalThemeUISystem` handles backend logic and state management
- **JavaScript UI**: `universal-theme-button.js` injects the button into the game's UI
- **Bindings**: Uses Colossal's UI binding system to communicate between C# and JavaScript
- **Dynamic Injection**: Button is automatically added when the zoning toolbar is opened

## Technical Details

### Framework:
- Target Framework: .NET Framework 4.8 (as configured in your project)
- Original repository targets: .NET Standard 2.1 (but adapted to work with .NET 4.8)

### Key Components:
1. **ZoneMergeSystem**: Runs during `SystemUpdatePhase.PrefabUpdate` to:
   - Create universal zone prefabs based on NA templates
   - Clone building prefabs and reassign them to universal zones
   - Apply probability filters based on user settings

2. **UniversalThemeUISystem**: Runs during `SystemUpdatePhase.UIUpdate` to:
   - Create a Universal theme entity
   - Manage UI state (active/inactive filtering)
   - Provide bindings for JavaScript UI interaction

3. **Settings System**: Provides in-game configuration UI with sliders for each building type

4. **Localization**: Full English language support for all settings, zone names, and UI elements

## Build Status
? Build successful - The project compiles without errors

## Next Steps for Publishing to PDX Mods

1. **Add a thumbnail image**:
   - Place an image at `Properties/Thumbnail.png`
   - Recommended size: 256x256 pixels
   - Consider using a purple/blue gradient theme to match the UI button

2. **Add screenshots** (optional but recommended):
   - Update the `<Screenshot Value="" />` lines in PublishConfiguration.xml
   - Show examples of mixed NA/EU neighborhoods
   - **Include a screenshot of the new Universal Theme button in action!**

3. **Test the mod**:
   - Build the project
   - Copy the DLL, mod.json, and ui_src folder to your local mods folder:
     `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\UniversalZoningSystem\`
   - Test in-game to ensure:
     - Universal zones appear and work correctly
     - The "UP" button appears in the zoning toolbar
     - Clicking the button filters to Universal zones only
     - Settings work as expected

4. **Publish to PDX Mods**:
   - Use the Cities: Skylines II mod tools to publish
   - The PublishConfiguration.xml contains all the necessary metadata
   - After first publish, save the ModId in PublishConfiguration.xml for future updates

## Important Notes

- The mod is set to `Public` access level in PublishConfiguration.xml
- The mod can be disabled by users (CanBeDisabled: true)
- Version is set to 1.0.0 - update this for future releases
- The GitHub repository link is included as an external link
- **UI Script**: The JavaScript UI extension is automatically loaded by the game when the mod loads

## UI Customization

If you want to customize the Universal Theme button appearance, edit `ui_src/universal-theme-button.js`:
- Change button colors in the `background` and `borderColor` style properties
- Modify the button text (currently "UP") in the `buttonIcon.innerHTML` line
- Adjust hover effects and animations
- Change button positioning

## Support Information

- Original Repository: https://github.com/redlabracer/Universal-Zoning-System--UZS-
- Original Author: redlabracer
- Enhanced by: Adding UI theme button feature
- License: Not specified in repository (consider adding one if publishing)
