# Universal Zoning System (UZS)

**Target Game:** Cities: Skylines II
**Mod Type:** Gameplay / Systems / UI
**Version:** 1.0 Draft

## Overview
The **Universal Zoning System (UZS)** addresses a restriction in the base game where zoning is strictly segregated by "Theme" (North American vs. European). This segregation forces players to manually toggle themes or paint specific districts to achieve architectural variety.

UZS creates a unified "Universal" zoning palette. When a player zones "Universal Low Density Residential," the game can randomly spawn a Victorian NA home next to a Modern EU cottage.

## Project Structure
*   `src/UniversalZoningSystem`: Main mod source code.
    *   `Mod.cs`: Entry point of the mod.
    *   `Systems/ZoneMergeSystem.cs`: Logic for merging assets into universal zones.
    *   `UniversalZoneDefinitions.cs`: Constants for the new zone types.

## Setup & Build

### Prerequisites
*   .NET 8.0 SDK
*   Cities: Skylines II installed (Steam version assumed)

### Configuration
1.  Open `src/UniversalZoningSystem/UniversalZoningSystem.csproj`.
2.  Update the `<GamePath>` property to point to your Cities: Skylines II `Managed` folder.
    *   Default: `C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II\Cities2_Data\Managed`

### Building
Run the following command in the root directory:
```bash
dotnet build
```

### Installation
To install the mod locally:
1.  Build the project.
2.  Copy the output DLL (`bin/Debug/net8.0/UniversalZoningSystem.dll`) to your local mods folder.
    *   Local Mods Path: `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\UniversalZoningSystem\` (Create the folder if it doesn't exist).
3.  Enable the mod in the game's main menu (if using a mod loader or developer mode).

## Development Notes
*   This project references game DLLs that are not included in the repository. You must have the game installed to build.
*   The `ZoneMergeSystem` is the core logic that scans for building prefabs and assigns them to the new Universal Zones.
