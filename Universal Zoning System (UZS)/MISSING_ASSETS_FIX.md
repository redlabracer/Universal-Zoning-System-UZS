# Missing Building Assets - Fix Applied

## Problem Description
After changing mod settings and restarting the game, some building assets were missing from saved games. This occurred because the mod's probability settings were controlling whether building prefabs were created, not just their spawn rates.

## Root Cause
The mod previously used the probability sliders (0-100%) to determine if a building variant should be cloned into the universal zone pool. When you changed these settings:
- Buildings that were previously created (e.g., at 50% probability) might not be created again (if you set to 0%)
- Save games would reference these "missing" buildings, causing visual glitches

## Fix Applied
1. **All building variants are now always created** regardless of probability settings
2. **Probability settings are currently disabled** - they no longer affect building creation
3. This ensures save game compatibility and prevents missing assets

## For Users
- **If you're experiencing missing buildings**: This fix should resolve the issue
- **Your existing settings will be preserved** but won't affect which buildings appear
- **Start a new game or load an existing save** - all building variants should now appear
- Note: The probability sliders in settings are kept for future use but currently don't affect gameplay

## For Mod Developers
The probability settings should ideally control **spawn weighting** (how often a building appears), not **prefab creation** (whether the building exists at all). A future update could implement proper spawn weighting using the game's zone spawn system.

### Technical Details
Changes made in `ZoneMergeSystem.cs`:
- `GetProbability()` now always returns 100
- Removed probability check in `ProcessMergeBatch()` that was skipping building creation
- All buildings are cloned regardless of user settings

## Recommendations
If missing assets persist:
1. Disable the mod, load your save, save again
2. Re-enable the mod and start a fresh game
3. Check the game's Player.log for any UZS-related errors
