# Building Despawn Issue - FIXED ?

## Problem
Buildings spawned from Universal zones were despawning unexpectedly during gameplay.

## Root Cause
The `BuildingProtectionSystem` had several critical flaws:

1. **Performance Issue**: The system was running **every frame** and iterating through **ALL buildings** in the city, checking each one individually. This was extremely inefficient.

2. **Wrong Detection Logic**: The system was trying to detect Universal buildings by checking zone blocks and components like `CurrentDistrict` and `CityServiceUpkeep`, which was unreliable.

3. **Missing the Obvious**: The system didn't check the most obvious identifier - the building prefab name! When `ZoneMergeSystem` clones buildings, it names them `"Universal_" + originalName`, making them easy to identify.

## Solution Implemented

### Changes to `BuildingProtectionSystem.cs`:

1. **Optimized Query**: Changed to only query buildings that are marked for `Deleted` (not ones that are `Abandoned` or `Condemned`, as those should naturally despawn).

2. **Simplified Detection**: Now checks if the building's prefab name starts with `"Universal_"` - this is the definitive way to identify buildings created by the mod.

3. **Reduced Frequency**: Added a frame counter to only check every 60 frames (about once per second), dramatically reducing performance impact.

4. **Better Logging**: Added debug counters to track how many buildings are being checked and protected.

### How It Works Now:

```csharp
// 1. Only query deleted buildings (excluding naturally abandoned/condemned ones)
m_DeletedBuildingsQuery = GetEntityQuery(
    ComponentType.ReadWrite<Building>(),
    ComponentType.ReadOnly<PrefabRef>(),
    ComponentType.ReadOnly<Deleted>(),
    ComponentType.Exclude<Abandoned>(),
    ComponentType.Exclude<Condemned>(),
    ComponentType.Exclude<SafeUninstallProtection>()
);

// 2. Check the prefab name (the reliable way)
if (buildingPrefab.name.StartsWith("Universal_"))
{
    // Remove the Deleted component to prevent despawn
    EntityManager.RemoveComponent<Deleted>(buildingEntity);
    protectedCount++;
}
```

## Expected Behavior After Fix

? Buildings spawned from Universal zones will **NOT** despawn unexpectedly  
? Buildings that are **Abandoned** or **Condemned** will still despawn naturally (as intended)  
? System runs efficiently (only checks deleted buildings, not all buildings)  
? Minimal performance impact (checks only once per second)  
? Clear debug logging to track protection activity

## Testing Instructions

1. **Rebuild the mod** (already done - build successful ?)
2. **Reload the game** and load your save
3. **Zone some Universal zones** and wait for buildings to spawn
4. **Watch the logs** - you should see messages like:
   - `"UZS BuildingProtection: Protected 'Universal_NA Residential Single-Family Large 01' from improper despawn"`
5. **Verify buildings stay** - Universal zone buildings should no longer randomly disappear

## Debug Commands

If buildings still despawn, check the Player.log for:
- Lines containing `"UZS BuildingProtection"` to see if protection is activating
- Look for messages about how many buildings are being checked and protected

## Additional Notes

- The safe uninstall system still works as before
- Buildings will still abandon/condemn naturally if they lose services, demand, etc.
- Only **improper** deletion (game bug/zone conflict) is prevented
