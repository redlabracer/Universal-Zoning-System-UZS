# Universal Theme Button - Feature Documentation

## Overview

The Universal Theme Button is a UI enhancement that adds a quick-access button to the zoning toolbar, allowing players to instantly filter and show only Universal zones.

## Visual Design

**Button Appearance:**
- **Label**: "UP" (Universal Pack)
- **Colors**: Purple gradient (RGB: #6B46C1 to #4299E1)
- **Border**: 2px solid purple (#805AD5)
- **Size**: Compact button that matches the game's theme button style
- **Position**: In the theme section, before the NA/EU theme buttons

**States:**
- **Inactive (Default)**: Bright purple gradient with light border
- **Active (Filtering)**: Darker purple gradient
- **Hover**: Scales up 5% with purple glow effect

## How It Works

### User Experience:
1. Player opens the zoning tool
2. The "UP" button appears automatically in the theme section
3. Clicking the button:
   - **First click**: Filters to show only Universal zones, dims other theme buttons
   - **Second click**: Restores all zones to visibility

### Technical Flow:
```
Player Click ? JavaScript Handler ? C# Binding ? System Update ? UI Update
```

1. **JavaScript** (`universal-theme-button.js`):
   - Injects button into DOM when zoning panel opens
   - Handles click events and visual state changes
   - Filters zone items by checking for "Universal" in names
   - Triggers C# binding for backend state management

2. **C# System** (`UniversalThemeUISystem.cs`):
   - Maintains filtering state
   - Provides bindings for JavaScript communication
   - Can be extended for additional backend logic

## Implementation Details

### Files Involved:
- `ui_src/universal-theme-button.js` - UI injection and interaction logic
- `Systems/UniversalThemeUISystem.cs` - Backend system for state management
- `Mod.cs` - System registration
- `LocaleEN.cs` - Localization strings
- `mod.json` - UI script reference

### JavaScript Functions:

#### `createUniversalThemeButton()`
Creates and injects the button into the DOM
- Finds the theme section container
- Creates styled button element
- Adds event listeners
- Inserts into correct position

#### `filterToUniversalZones()`
Applies the Universal filter
- Hides all non-Universal zone items
- Dims theme buttons except Universal
- Updates visual feedback

#### `showAllZones()`
Removes the filter
- Shows all zone items
- Restores theme button opacity
- Resets visual state

### C# Bindings:

#### `isUniversalThemeActive`
ValueBinding<bool> - Tracks whether Universal filtering is active

#### `toggleUniversalTheme`
TriggerBinding - Called when button is clicked

## Customization Guide

### Changing Button Colors:
Edit `ui_src/universal-theme-button.js`, line ~20:
```javascript
background: linear-gradient(135deg, #YOUR_COLOR1 0%, #YOUR_COLOR2 100%);
border: 2px solid #YOUR_BORDER_COLOR;
```

### Changing Button Text:
Edit `ui_src/universal-theme-button.js`, line ~35:
```javascript
buttonIcon.innerHTML = 'YOUR_TEXT';
```

### Changing Button Position:
Modify the insertion logic in `createUniversalThemeButton()`:
```javascript
// Insert at beginning
themeSection.insertBefore(buttonContainer, themeSection.firstChild);

// Insert at end
themeSection.appendChild(buttonContainer);
```

## Browser Compatibility

The UI extension uses standard JavaScript features compatible with:
- Chromium Embedded Framework (CEF) - Used by Cities: Skylines II
- Modern browsers (Chrome, Edge, Firefox)

## Troubleshooting

### Button doesn't appear:
- Check browser console for errors
- Verify `mod.json` has correct `UIScript` path
- Ensure `ui_src` folder is copied to output directory

### Button appears but doesn't filter:
- Verify zone items have correct class names or titles
- Check that "Universal" zones have "Universal" in their names
- Review browser console for JavaScript errors

### Filtering removes all zones:
- Check that Universal zones were created successfully
- Verify zone naming matches the filter logic
- Look for log messages: "UZS: Created universal zone..."

## Future Enhancements

Potential improvements:
- Icon instead of text for the button
- Multiple filter modes (all, residential only, commercial only)
- Remember last filter state between sessions
- Keyboard shortcut to toggle filter
- Animation when transitioning between filtered/unfiltered states

## Testing Checklist

Before publishing:
- [ ] Button appears when opening zoning tool
- [ ] Button has correct visual styling
- [ ] Hover effect works properly
- [ ] Click toggles filter on/off
- [ ] Universal zones remain visible when filtered
- [ ] Other zones are hidden when filtered
- [ ] Button state persists while zoning tool is open
- [ ] No console errors in browser
- [ ] Works with different screen resolutions
- [ ] Compatible with other UI mods (test if possible)

## Credits

- Universal Zoning System base mod by redlabracer
- UI Theme Button feature enhancement
- Inspired by the theme selection shown in the game's zoning toolbar
