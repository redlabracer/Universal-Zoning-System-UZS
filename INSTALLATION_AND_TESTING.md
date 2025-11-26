# Quick Installation & Testing Guide

## Installation Steps

1. **Build the Project**
   - Open the solution in Visual Studio
   - Build in Debug or Release mode
   - Check for successful build

2. **Locate Build Output**
   - Navigate to: `bin\Debug\net48\` or `bin\Release\net48\`
   - You should find:
     - `Universal Zoning System (UZS).dll`
     - `mod.json`
     - `ui_src\universal-theme-button.js`

3. **Install to Game**
   - Open File Explorer
   - Navigate to: `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\`
   - Create folder: `UniversalZoningSystem`
   - Copy ALL files from build output to this folder:
     ```
     Mods\UniversalZoningSystem\
     ??? Universal Zoning System (UZS).dll
     ??? mod.json
     ??? ui_src\
         ??? universal-theme-button.js
     ```

4. **Launch Game**
   - Start Cities: Skylines II
   - The mod should load automatically

## Testing the Universal Theme Button

### Step 1: Verify Mod Loaded
1. Open the game's mod manager (if available)
2. Check that "Universal Zoning System (UZS)" is listed
3. Ensure it's enabled

### Step 2: Check Console Logs
1. Press F12 or open developer console (if available)
2. Look for these messages:
   ```
   UZS: Mod has been loaded!
   UZS: Created universal zone 'Universal_ResLow' from 'NA Residential Low'
   UZS: Universal Theme UI System registered
   UZS: UI extension loaded
   ```

### Step 3: Test Universal Zones
1. Start or load a city
2. Open the zoning tool
3. Look for new "Universal" zones in the palette:
   - Universal Low Density Residential
   - Universal Row Housing
   - Universal Medium Density Residential
   - etc.
4. Try placing some Universal zones
5. Verify they accept both NA and EU buildings

### Step 4: Test the Theme Button
1. Open the zoning tool
2. Look in the theme section (top area with theme buttons)
3. You should see a **purple "UP" button**
4. Click the button:
   - ? Button should darken
   - ? Only Universal zones should be visible
   - ? Theme buttons (NA, EU, etc.) should dim
5. Click again:
   - ? Button should brighten
   - ? All zones should be visible again
   - ? Theme buttons should restore

### Step 5: Test Settings
1. Open game settings
2. Find "Universal Zoning System" section
3. Test the probability sliders:
   - NA Residential sliders
   - EU Residential sliders
   - Commercial sliders
4. Verify changes save

## Troubleshooting

### Mod doesn't load
- **Check**: Is the DLL in the correct folder?
- **Check**: Is mod.json present?
- **Solution**: Verify folder structure matches installation steps

### Universal zones don't appear
- **Check**: Game logs for "UZS: Created universal zone..."
- **Check**: Are you using the zoning tool (not bulldoze/other tools)?
- **Solution**: Wait a few seconds after loading; zones create on startup

### Theme button doesn't appear
- **Check**: Is ui_src folder copied to game's mod folder?
- **Check**: Does mod.json have "UIScript" field?
- **Check**: Browser console for JavaScript errors
- **Solution**: Verify mod.json contains: `"UIScript": "ui_src/universal-theme-button.js"`

### Button appears but doesn't work
- **Check**: Browser console for errors
- **Check**: Are Universal zones actually created?
- **Try**: Restart the game
- **Solution**: Check that zone names include "Universal"

### No buildings spawn in Universal zones
- **Check**: Settings - are probabilities set to 0%?
- **Check**: Do you have both NA and EU DLCs/content?
- **Solution**: Increase probability sliders in mod settings

## Development Console Commands

If you have access to the development console:

```javascript
// Check if UI bindings are registered
engine.isRegistered("universalZoning.toggleUniversalTheme");

// Manually toggle the filter
engine.trigger("universalZoning.toggleUniversalTheme");

// Check current state
engine.getValue("universalZoning.isUniversalThemeActive");
```

## File Verification Checklist

Before testing, verify these files exist:

**In your mod folder:**
```
%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\UniversalZoningSystem\
??? ? Universal Zoning System (UZS).dll
??? ? mod.json
??? ? ui_src\
    ??? ? universal-theme-button.js
```

**mod.json must contain:**
```json
{
    "Name": "UniversalZoningSystem",
    "DisplayName": "Universal Zoning System (UZS)",
    "Version": "1.0.0",
    "Description": "Mix North American and European building styles...",
    "Author": "redlabracer",
    "CanBeDisabled": true,
    "UIScript": "ui_src/universal-theme-button.js"
}
```

## Expected Visual Result

When working correctly, you should see:
1. **Zoning Palette**: 8 new Universal zones with purple/blue indicators
2. **Theme Section**: A purple "UP" button next to NA/EU buttons
3. **Hover Effect**: Button grows slightly and glows when hovering
4. **Filter Active**: Only Universal zones visible, others hidden
5. **Buildings**: Mix of NA and EU architecture in Universal zones

## Performance Notes

- Mod loads during game startup (expect a few seconds)
- UI injection happens when zoning tool opens (instant)
- Filtering is instant (no lag)
- No impact on game performance when not using zoning tool

## Known Limitations

1. Theme button style may vary slightly depending on game UI updates
2. Filtering is visual only (doesn't modify game data)
3. Button position may shift if other UI mods modify the toolbar
4. Requires manual re-testing after game updates

## Next Steps After Testing

Once everything works:
1. Take screenshots of the button in action
2. Record a short video demo (optional)
3. Prepare thumbnail image (256x256px)
4. Update PublishConfiguration.xml with screenshots
5. Publish to PDX Mods!

## Support

If you encounter issues:
- Check game logs: `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Logs\`
- Review browser console (F12)
- Verify file structure
- Try disabling other mods to check for conflicts
