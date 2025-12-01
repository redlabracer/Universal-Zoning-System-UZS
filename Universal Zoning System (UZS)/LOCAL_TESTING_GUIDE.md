# Testing UZS v1.9.3 Locally

## Problem
The game is loading the OLD version (1.9.2) from Paradox Mods, not your fixed version.

## Solution: Test with Local Version

### Step 1: Copy Your Fixed DLL to Local Mods Folder
```powershell
# Create local mods directory if it doesn't exist
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\.cache\Mods\mods_workInProgress\UniversalZoningSystem"

# Copy your fixed DLL
Copy-Item "C:\Users\charl\source\repos\Universal Zoning System\Universal Zoning System (UZS)\Universal Zoning System (UZS)\bin\Release\net48\*" -Destination "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\.cache\Mods\mods_workInProgress\UniversalZoningSystem" -Force

# Copy mod.json too
Copy-Item "C:\Users\charl\source\repos\Universal Zoning System\Universal Zoning System (UZS)\Universal Zoning System (UZS)\mod.json" -Destination "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\.cache\Mods\mods_workInProgress\UniversalZoningSystem" -Force
```

### Step 2: Disable the Published Version
1. Open Cities: Skylines II
2. Go to **Mods** menu
3. **DISABLE** "Universal Zoning System (UZS)" from Paradox Mods (version 1.9.2)
4. **ENABLE** the local development version in `mods_workInProgress`

### Step 3: Test Your Save
1. Load your save game
2. Check Player.log for: `"UZS: Detected existing save game with 8 Universal zones already present. Skipping zone creation..."`
3. Verify no crash occurs

### Step 4: If It Works
Then you can publish version 1.9.3 to Paradox Mods:
```powershell
cd "C:\Users\charl\source\repos\Universal Zoning System\Universal Zoning System (UZS)\Universal Zoning System (UZS)"
dotnet publish -c Release /p:PublishProfile=PublishNewVersion
```

## What to Look For in Player.log

### ? GOOD (Fix Working):
```
UZS: New game or first-time load detected. Will create Universal zones.
(only on first load)

or

UZS: Detected existing save game with 8 Universal zones already present. Skipping zone creation...
(on subsequent loads)
```

### ? BAD (Old Version):
```
UZS: Created universal zone 'Universal_ResLow'...
UZS: Starting merge process for 8057 entities...
UZS: Finished cloning. Total cloned: 6980.
(every time you load)
```

## Quick Test Commands
```powershell
# Copy fixed version to local mods
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\.cache\Mods\mods_workInProgress\UniversalZoningSystem"
Copy-Item "C:\Users\charl\source\repos\Universal Zoning System\Universal Zoning System (UZS)\Universal Zoning System (UZS)\bin\Release\net48\*" -Destination "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\.cache\Mods\mods_workInProgress\UniversalZoningSystem" -Force
Copy-Item "C:\Users\charl\source\repos\Universal Zoning System\Universal Zoning System (UZS)\Universal Zoning System (UZS)\mod.json" -Destination "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\.cache\Mods\mods_workInProgress\UniversalZoningSystem" -Force

# Check logs after testing
Select-String -Path "$env:USERPROFILE\AppData\LocalLow\Colossal Order\Cities Skylines II\Player.log" -Pattern "UZS:" | Select-Object -Last 10
```
