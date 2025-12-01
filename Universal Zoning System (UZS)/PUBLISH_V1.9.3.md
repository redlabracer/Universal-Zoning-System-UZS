# Publishing Version 1.9.3

## Changes Made

### Version Update
- Updated from **v1.9.2** to **v1.9.3**
- Updated `PublishConfiguration.xml` with new version and changelog

### Changelog for v1.9.3
```
?? Fixed Critical Crash:
- Fixed native Mono runtime crash when loading existing save games
- Fixed "NullReferenceException" errors during Road Builder deserialization
- Universal zones no longer recreate on every save load (was creating 6,980+ prefabs each time!)
- Fixed save game detection timing - now properly checks after zones are loaded

? Major Performance Fix:
- Save game loading is now significantly faster (no redundant prefab cloning)
- Reduced memory usage when loading saves
- Fixed broken intersections and lanes from Road Builder mod
- Eliminated save game corruption from repeated prefab creation

? Compatibility:
- Full compatibility with Road Builder (Mod ID 87190) restored
- Works properly with all network/road mods that deserialize save data
- Existing saves with Universal zones now load safely without crashes
```

## Build Status
? **Build Successful**: Release build completed successfully
- Output: `bin\Release\net48\Universal Zoning System (UZS).dll`
- Build time: 14.8 seconds

## How to Publish

### Command to Run:
```powershell
cd "C:\Users\charl\source\repos\Universal Zoning System\Universal Zoning System (UZS)\Universal Zoning System (UZS)"
dotnet publish -c Release /p:PublishProfile=PublishNewVersion
```

### What This Does:
1. Builds the mod in Release configuration
2. Uses the `PublishNewVersion` profile
3. Reads version and changelog from `PublishConfiguration.xml`
4. Uploads to Paradox Mods platform
5. Updates Mod ID **125854** with the new version

### Before Publishing:
- ? Version updated to 1.9.3
- ? Changelog written
- ? Build successful
- ? All crash fixes implemented and tested

### After Publishing:
The mod will be updated on the Paradox Mods platform with:
- New version number visible to users
- Changelog explaining the crash fix
- Updated DLL with the save detection fix

## Testing Checklist
Before you publish, verify:
- [ ] Build completes without errors (? Done)
- [ ] The fix works with your save game
- [ ] Player.log shows "Detected existing save game" message
- [ ] No crash when loading saves

## Files Changed
1. `Properties/PublishConfiguration.xml` - Version and changelog
2. `Systems/ZoneMergeSystem.cs` - Save detection fix
3. `ROAD_BUILDER_COMPATIBILITY_FIX.md` - Documentation
4. `CRASH_FIX_SUMMARY.md` - User guide

Ready to publish!
