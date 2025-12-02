using Game;
using Game.Common;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalZoningSystem.Systems
{
    public partial class ZoneMergeSystem : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem = null!;
        private int m_State = 0;
        private Dictionary<string, ZonePrefab> m_CreatedPrefabs = new Dictionary<string, ZonePrefab>();
        private Dictionary<string, Entity> m_UniversalZoneEntities = new Dictionary<string, Entity>();

        // Batch processing fields
        private NativeArray<Entity> m_EntitiesToProcess;
        private int m_ProcessIndex = 0;
        private const int BATCH_SIZE = 20;
        private int m_ClonedCount = 0;
        private HashSet<string> m_ExistingPrefabNames = new HashSet<string>();

        // Add delay to ensure AssetDatabase has finished initialization
        private float m_InitialDelay = 5.0f;
        private double m_StartTime = 0;
        private int m_ReadyCheckAttempts = 0;
        private const int MAX_READY_CHECK_ATTEMPTS = 300;
        private const float READY_CHECK_INTERVAL = 0.1f;
        private double m_LastReadyCheck = 0;

        // Store template zone data for post-initialization
        private Dictionary<string, ZoneData> m_TemplateZoneData = new Dictionary<string, ZoneData>();

        // Optimization: Cache zone info
        private struct ZoneMergeInfo
        {
            public string TargetUniversalZone;
            public int Probability;
        }
        private Dictionary<Entity, ZoneMergeInfo> m_ZoneMergeInfos = new Dictionary<Entity, ZoneMergeInfo>();

        // Add flag to detect if we're loading a save game
        private bool m_IsLoadingExistingSave = false;
        private bool m_HasCheckedSaveState = false;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_StartTime = World.Time.ElapsedTime;
            m_LastReadyCheck = m_StartTime;
        }

        protected override void OnDestroy()
        {
            if (m_EntitiesToProcess.IsCreated)
            {
                m_EntitiesToProcess.Dispose();
            }
            
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            // Wait for initial delay to ensure AssetDatabase is stable
            if (World.Time.ElapsedTime - m_StartTime < m_InitialDelay)
            {
                return;
            }

            if (m_State == 0)
            {
                // Wait for prefabs to be loaded and ensure AssetDatabase is ready
                var query = GetEntityQuery(ComponentType.ReadOnly<PrefabData>(), ComponentType.ReadOnly<ZoneData>());
                int zoneCount = query.CalculateEntityCount();
                
                if (zoneCount > 50)
                {
                    // NOW check if we're loading an existing save (only check once, after zones are loaded)
                    if (!m_HasCheckedSaveState)
                    {
                        m_HasCheckedSaveState = true;
                        
                        // Check if Universal zones already exist - if they do, we're loading a save that already had UZS
                        var existingUniversalZones = CheckForExistingUniversalZones();
                        
                        if (existingUniversalZones > 0)
                        {
                            UnityEngine.Debug.Log($"UZS: Detected existing save game with {existingUniversalZones} Universal zones already present. Skipping zone creation to preserve save game integrity and Road Builder compatibility.");
                            m_IsLoadingExistingSave = true;
                            m_State = 2;
                            Enabled = false;
                            return;
                        }
                        
                        UnityEngine.Debug.Log("UZS: New game or first-time load detected. Will create Universal zones.");
                    }
                    
                    // If we detected we're loading an existing save, don't do anything
                    if (m_IsLoadingExistingSave)
                    {
                        return;
                    }
                    
                    UnityEngine.Debug.Log($"UZS: Starting zone creation with {zoneCount} existing zones after {World.Time.ElapsedTime - m_StartTime:F1}s delay");
                    CreateUniversalZones();
                    m_State = 1;
                    m_ReadyCheckAttempts = 0;
                    m_LastReadyCheck = World.Time.ElapsedTime;
                }
                else
                {
                    // Log progress every 60 frames to help diagnose startup issues
                    if (((int)(World.Time.ElapsedTime * 60)) % 60 == 0)
                    {
                        UnityEngine.Debug.Log($"UZS: Waiting for game initialization... Zone count: {zoneCount}/50");
                    }
                }
            }
            else if (m_State == 1)
            {
                // Rate limit ready checks
                if (World.Time.ElapsedTime - m_LastReadyCheck < READY_CHECK_INTERVAL)
                {
                    return;
                }
                
                m_LastReadyCheck = World.Time.ElapsedTime;
                m_ReadyCheckAttempts++;
                
                if (CheckUniversalZonesReady())
                {
                    UnityEngine.Debug.Log($"UZS: All universal zones ready after {m_ReadyCheckAttempts} attempts");
                    StartMerge();
                    ProcessMergeBatch();
                    m_State = 2;
                    Enabled = false;
                }
                else if (m_ReadyCheckAttempts >= MAX_READY_CHECK_ATTEMPTS)
                {
                    UnityEngine.Debug.LogError($"UZS: Universal zones failed to initialize after {MAX_READY_CHECK_ATTEMPTS} attempts ({MAX_READY_CHECK_ATTEMPTS * READY_CHECK_INTERVAL}s). System disabled.");
                    UnityEngine.Debug.LogError($"UZS: Created zones: {m_CreatedPrefabs.Count}, Ready zones: {m_UniversalZoneEntities.Count}");
                    
                    // Try to fix zones with invalid AreaType by copying from template
                    FixInvalidZones();
                    
                    // Log which zones failed to initialize
                    foreach (var kvp in m_CreatedPrefabs)
                    {
                        if (!m_UniversalZoneEntities.ContainsKey(kvp.Key))
                        {
                            Entity e = m_PrefabSystem.GetEntity(kvp.Value);
                            if (e == Entity.Null)
                            {
                                UnityEngine.Debug.LogError($"UZS: Zone '{kvp.Key}' has null entity");
                            }
                            else if (!EntityManager.HasComponent<ZoneData>(e))
                            {
                                UnityEngine.Debug.LogError($"UZS: Zone '{kvp.Key}' missing ZoneData component");
                            }
                            else
                            {
                                var zoneData = EntityManager.GetComponentData<ZoneData>(e);
                                UnityEngine.Debug.LogError($"UZS: Zone '{kvp.Key}' has invalid AreaType: {zoneData.m_AreaType}");
                            }
                        }
                    }
                    
                    m_State = 2;
                    Enabled = false;
                }
                else if (m_ReadyCheckAttempts % 20 == 0)
                {
                    UnityEngine.Debug.Log($"UZS: Waiting for universal zones... Attempt {m_ReadyCheckAttempts}/{MAX_READY_CHECK_ATTEMPTS}, Ready: {m_UniversalZoneEntities.Count}/{m_CreatedPrefabs.Count}");
                }
            }
        }

        private int CheckForExistingUniversalZones()
        {
            int zoneCount = 0;
            int buildingCount = 0;
            
            // Check for Universal zones
            var zoneQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>(), ComponentType.ReadOnly<ZoneData>());
            var zoneEntities = zoneQuery.ToEntityArray(Allocator.Temp);
            
            foreach (var entity in zoneEntities)
            {
                var prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
                if (prefab != null && prefab.name.StartsWith("Universal_"))
                {
                    zoneCount++;
                }
            }
            zoneEntities.Dispose();
            
            // CRITICAL: Also check for Universal buildings
            // If zones exist but buildings don't, we need to recreate them
            if (zoneCount > 0)
            {
                var buildingQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>(), ComponentType.ReadOnly<SpawnableBuildingData>());
                var buildingEntities = buildingQuery.ToEntityArray(Allocator.Temp);
                
                foreach (var entity in buildingEntities)
                {
                    var prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
                    if (prefab != null && prefab.name.StartsWith("Universal_"))
                    {
                        buildingCount++;
                    }
                }
                buildingEntities.Dispose();
                
                UnityEngine.Debug.Log($"UZS: Found {zoneCount} Universal zones and {buildingCount} Universal buildings");
                
                // Only skip if we have BOTH zones AND a reasonable number of buildings
                // If buildings are missing, we need to recreate them
                if (buildingCount < 100)
                {
                    UnityEngine.Debug.Log($"UZS: Universal zones exist but only {buildingCount} buildings found (expected ~6980). Will recreate buildings.");
                    return 0; // Return 0 to trigger recreation
                }
            }
            
            return zoneCount;
        }

        private void CreateUniversalZones()
        {
            UnityEngine.Debug.Log("UZS: Beginning Universal Zone creation...");
            
            // Build lookup of existing zones
            var zoneLookup = new Dictionary<string, Entity>();
            
            var zoneQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>(), ComponentType.ReadOnly<ZoneData>());
            var zoneEntities = zoneQuery.ToEntityArray(Allocator.Temp);
            
            foreach (var entity in zoneEntities)
            {
                var prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
                if (prefab != null)
                {
                    if (!zoneLookup.ContainsKey(prefab.name))
                    {
                        zoneLookup.Add(prefab.name, entity);
                        var zoneData = EntityManager.GetComponentData<ZoneData>(entity);
                        UnityEngine.Debug.Log($"UZS: Found zone '{prefab.name}' with AreaType: {zoneData.m_AreaType}");
                    }
                }
            }
            zoneEntities.Dispose();

            var zonesToCreate = new Dictionary<string, string>
            {
                { UniversalZoneDefinitions.Universal_ResLow, "NA Residential Low" },
                { UniversalZoneDefinitions.Universal_ResRow, "NA Residential Medium Row" },
                { UniversalZoneDefinitions.Universal_ResMed, "NA Residential Medium" },
                { UniversalZoneDefinitions.Universal_ResHigh, "NA Residential High" },
                { UniversalZoneDefinitions.Universal_ResLowRent, "Residential LowRent" },
                { UniversalZoneDefinitions.Universal_ComLow, "NA Commercial Low" },
                { UniversalZoneDefinitions.Universal_ComHigh, "NA Commercial High" },
                { UniversalZoneDefinitions.Universal_Mixed, "NA Residential Mixed" }
            };

            int createdCount = 0;
            foreach (var kvp in zonesToCreate)
            {
                Entity templateEntity = Entity.Null;
                if (zoneLookup.TryGetValue(kvp.Value, out templateEntity))
                {
                    var sourcePrefab = m_PrefabSystem.GetPrefab<ZonePrefab>(templateEntity);
                    if (sourcePrefab != null)
                    {
                        // Verify template has ZoneData before cloning
                        if (!EntityManager.HasComponent<ZoneData>(templateEntity))
                        {
                            UnityEngine.Debug.LogWarning($"UZS: Template zone '{kvp.Value}' missing ZoneData component, skipping '{kvp.Key}'");
                            continue;
                        }

                        // Get and store the source zone data
                        var sourceZoneData = EntityManager.GetComponentData<ZoneData>(templateEntity);
                        if (sourceZoneData.m_AreaType == Game.Zones.AreaType.None)
                        {
                            UnityEngine.Debug.LogWarning($"UZS: Template zone '{kvp.Value}' has invalid AreaType.None, skipping '{kvp.Key}'");
                            continue;
                        }

                        // Store template zone data for later use
                        m_TemplateZoneData[kvp.Key] = sourceZoneData;

                        try
                        {
                            // Create the new Universal Zone based on the NA template
                            var newPrefab = Object.Instantiate(sourcePrefab);
                            newPrefab.name = kvp.Key;
                            
                            m_PrefabSystem.AddPrefab(newPrefab);
                            m_CreatedPrefabs[kvp.Key] = newPrefab;
                            createdCount++;
                            UnityEngine.Debug.Log($"UZS: Created universal zone '{kvp.Key}' from '{kvp.Value}' with AreaType: {sourceZoneData.m_AreaType}");
                        }
                        catch (System.Exception ex)
                        {
                            UnityEngine.Debug.LogError($"UZS: Failed to create zone '{kvp.Key}': {ex.Message}");
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"UZS: Could not find template zone '{kvp.Value}' for '{kvp.Key}'");
                }
            }
            
            UnityEngine.Debug.Log($"UZS: Zone creation complete. Created {createdCount}/{zonesToCreate.Count} zones.");
        }

        private void FixInvalidZones()
        {
            UnityEngine.Debug.Log("UZS: Attempting to fix zones with invalid AreaType...");
            int fixedCount = 0;

            foreach (var kvp in m_CreatedPrefabs)
            {
                Entity e = m_PrefabSystem.GetEntity(kvp.Value);
                if (e != Entity.Null && EntityManager.HasComponent<ZoneData>(e))
                {
                    var zoneData = EntityManager.GetComponentData<ZoneData>(e);
                    
                    // If zone has invalid AreaType and we have template data, apply it
                    if (zoneData.m_AreaType == Game.Zones.AreaType.None && m_TemplateZoneData.ContainsKey(kvp.Key))
                    {
                        var templateData = m_TemplateZoneData[kvp.Key];
                        EntityManager.SetComponentData(e, templateData);
                        fixedCount++;
                        UnityEngine.Debug.Log($"UZS: Fixed zone '{kvp.Key}' by applying template AreaType: {templateData.m_AreaType}");
                        
                        // Add to ready list
                        m_UniversalZoneEntities[kvp.Key] = e;
                    }
                }
            }

            if (fixedCount > 0)
            {
                UnityEngine.Debug.Log($"UZS: Fixed {fixedCount} zones with invalid AreaType");
            }
        }

        private bool CheckUniversalZonesReady()
        {
            if (m_CreatedPrefabs.Count == 0)
            {
                UnityEngine.Debug.LogWarning("UZS: No zones were created to check");
                return false;
            }
            
            bool allReady = true;
            int readyCount = 0;
            
            foreach(var kvp in m_CreatedPrefabs)
            {
                // Skip if already validated
                if (m_UniversalZoneEntities.ContainsKey(kvp.Key))
                {
                    readyCount++;
                    continue;
                }
                
                Entity e = m_PrefabSystem.GetEntity(kvp.Value);
                if (e == Entity.Null)
                {
                    allReady = false;
                    continue;
                }
                
                // Verify the entity has ZoneData
                if (!EntityManager.HasComponent<ZoneData>(e))
                {
                    allReady = false;
                    continue;
                }
                
                // Check if ZoneData has valid AreaType
                var zoneData = EntityManager.GetComponentData<ZoneData>(e);
                if (zoneData.m_AreaType == Game.Zones.AreaType.None)
                {
                    // Try to fix immediately if we have template data
                    if (m_TemplateZoneData.ContainsKey(kvp.Key))
                    {
                        var templateData = m_TemplateZoneData[kvp.Key];
                        EntityManager.SetComponentData(e, templateData);
                        UnityEngine.Debug.Log($"UZS: Fixed zone '{kvp.Key}' by applying template AreaType: {templateData.m_AreaType}");
                        
                        // Verify the fix worked
                        zoneData = EntityManager.GetComponentData<ZoneData>(e);
                        if (zoneData.m_AreaType != Game.Zones.AreaType.None)
                        {
                            m_UniversalZoneEntities[kvp.Key] = e;
                            readyCount++;
                            continue;
                        }
                    }
                    
                    // Still invalid after fix attempt
                    if (m_ReadyCheckAttempts == 1 || m_ReadyCheckAttempts % 50 == 0)
                    {
                        UnityEngine.Debug.LogWarning($"UZS: Universal zone '{kvp.Key}' has invalid AreaType.None (attempt {m_ReadyCheckAttempts})");
                    }
                    allReady = false;
                    continue;
                }
                
                // Zone is valid - cache it
                if (m_ReadyCheckAttempts == 1 || !m_UniversalZoneEntities.ContainsKey(kvp.Key))
                {
                    UnityEngine.Debug.Log($"UZS: Zone '{kvp.Key}' ready with AreaType: {zoneData.m_AreaType}");
                }
                m_UniversalZoneEntities[kvp.Key] = e;
                readyCount++;
            }
            
            // Log progress
            if (m_ReadyCheckAttempts % 20 == 0 && !allReady)
            {
                UnityEngine.Debug.Log($"UZS: Ready check: {readyCount}/{m_CreatedPrefabs.Count} zones ready");
            }
            
            return allReady;
        }

        private void StartMerge()
        {
            // Build a cache of existing prefab names to avoid duplicates
            m_ExistingPrefabNames.Clear();
            var allPrefabsQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>());
            var allPrefabEntities = allPrefabsQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in allPrefabEntities)
            {
                var prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
                if (prefab != null)
                {
                    m_ExistingPrefabNames.Add(prefab.name);
                }
            }
            allPrefabEntities.Dispose();

            // Query for all entities that are spawnable buildings and have prefab data
            // Exclude Signature Buildings to prevent duplication of unique buildings
            var query = GetEntityQuery(
                ComponentType.ReadOnly<SpawnableBuildingData>(), 
                ComponentType.ReadOnly<PrefabData>(),
                ComponentType.Exclude<SignatureBuildingData>()
            );
            
            // Store entities in a persistent array to process over multiple frames
            m_EntitiesToProcess = query.ToEntityArray(Allocator.Persistent);
            
            m_ProcessIndex = 0;
            m_ClonedCount = 0;
            
            BuildZoneCache();

            UnityEngine.Debug.Log($"UZS: Starting merge process for {m_EntitiesToProcess.Length} entities. Existing prefabs: {m_ExistingPrefabNames.Count}");
        }

        private void ProcessMergeBatch()
        {
            if (!m_EntitiesToProcess.IsCreated) return;

            UnityEngine.Debug.Log($"UZS: Starting synchronous merge of {m_EntitiesToProcess.Length} entities...");
            long startTime = System.Diagnostics.Stopwatch.GetTimestamp();

            int skippedAlreadyUniversal = 0;
            int skippedAlreadyExists = 0;
            int skippedNoMatch = 0;

            while (m_ProcessIndex < m_EntitiesToProcess.Length)
            {
                Entity entity = m_EntitiesToProcess[m_ProcessIndex];
                m_ProcessIndex++;
                
                // Get the managed prefab associated with this entity
                var prefab = m_PrefabSystem.GetPrefab<BuildingPrefab>(entity);
                if (prefab == null) continue;

                // CRITICAL FIX: Skip if already a Universal clone to prevent exponential growth
                if (prefab.name.StartsWith("Universal_"))
                {
                    skippedAlreadyUniversal++;
                    continue;
                }

                // CRITICAL FIX: Skip if the target Universal prefab already exists
                if (m_ExistingPrefabNames.Contains("Universal_" + prefab.name))
                {
                    skippedAlreadyExists++;
                    continue;
                }

                var spawnableData = EntityManager.GetComponentData<SpawnableBuildingData>(entity);
                Entity zoneEntity = spawnableData.m_ZonePrefab;
                
                if (zoneEntity != Entity.Null)
                {
                    // Optimization: Use cached zone info
                    if (m_ZoneMergeInfos.TryGetValue(zoneEntity, out ZoneMergeInfo info))
                    {
                        if (m_CreatedPrefabs.TryGetValue(info.TargetUniversalZone, out ZonePrefab universalZonePrefab))
                        {
                            try
                            {
                                // Clone the managed prefab
                                var newPrefab = Object.Instantiate(prefab);
                                newPrefab.name = "Universal_" + prefab.name;
                                
                                // Update the SpawnableBuilding component on the managed prefab
                                // This ensures the entity is created with the correct zone from the start
                                if (newPrefab.TryGet<SpawnableBuilding>(out var spawnable))
                                {
                                    spawnable.m_ZoneType = universalZonePrefab;
                                    
                                    // Register it with the system
                                    m_PrefabSystem.AddPrefab(newPrefab);
                                    m_ExistingPrefabNames.Add(newPrefab.name); // Add to cache
                                    m_ClonedCount++;
                                }
                                else
                                {
                                    UnityEngine.Debug.LogWarning($"UZS: Could not find SpawnableBuilding component on cloned prefab {newPrefab.name}");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                UnityEngine.Debug.LogError($"UZS: Failed to clone building '{prefab.name}': {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        skippedNoMatch++;
                    }
                }
            }

            double elapsed = (System.Diagnostics.Stopwatch.GetTimestamp() - startTime) / (double)System.Diagnostics.Stopwatch.Frequency;
            UnityEngine.Debug.Log($"UZS: Finished cloning. Total cloned: {m_ClonedCount}. " +
                $"Skipped: {skippedAlreadyUniversal} already universal, {skippedAlreadyExists} already exists, " +
                $"{skippedNoMatch} no zone match. Time taken: {elapsed:F2} seconds.");
            
            m_EntitiesToProcess.Dispose();
        }

        private void BuildZoneCache()
        {
            m_ZoneMergeInfos.Clear();
            var zoneQuery = GetEntityQuery(ComponentType.ReadOnly<ZoneData>(), ComponentType.ReadOnly<PrefabData>());
            var zoneEntities = zoneQuery.ToEntityArray(Allocator.Temp);
            
            Setting? setting = Mod.Instance?.Setting;

            foreach (var entity in zoneEntities)
            {
                var prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
                if (prefab != null)
                {
                    string zoneName = prefab.name;
                    string? target = GetTargetUniversalZone(zoneName);
                    
                    if (target != null)
                    {
                        m_ZoneMergeInfos[entity] = new ZoneMergeInfo
                        {
                            TargetUniversalZone = target,
                            Probability = GetProbability(zoneName, setting)
                        };
                    }
                    else
                    {
                        // Log unmapped zones to help debug missing region packs
                        if (!zoneName.StartsWith("Universal_") && !zoneName.Contains("Office") && !zoneName.Contains("Industrial") && !zoneName.Contains("Extractor") && !zoneName.Contains("Service"))
                        {
                            UnityEngine.Debug.LogWarning($"UZS: Could not map zone '{zoneName}' to any universal zone.");
                        }
                    }
                }
            }
            zoneEntities.Dispose();
            UnityEngine.Debug.Log($"UZS: Built zone cache with {m_ZoneMergeInfos.Count} entries.");
        }

        private string? GetTargetUniversalZone(string zoneName)
        {
            string lowerName = zoneName.ToLowerInvariant();

            if (lowerName.Contains("residential"))
            {
                if (lowerName.Contains("row"))
                    return UniversalZoneDefinitions.Universal_ResRow;

                if (lowerName.Contains("mixed"))
                    return UniversalZoneDefinitions.Universal_Mixed;

                if (lowerName.Contains("low") && !lowerName.Contains("rent"))
                    return UniversalZoneDefinitions.Universal_ResLow;
                
                if (lowerName.Contains("medium"))
                    return UniversalZoneDefinitions.Universal_ResMed;
                
                if (lowerName.Contains("high"))
                    return UniversalZoneDefinitions.Universal_ResHigh;
                
                if (lowerName.Contains("lowrent") || (lowerName.Contains("low") && lowerName.Contains("rent")))
                    return UniversalZoneDefinitions.Universal_ResLowRent;
            }
            else if (lowerName.Contains("commercial"))
            {
                if (lowerName.Contains("low"))
                    return UniversalZoneDefinitions.Universal_ComLow;
                
                if (lowerName.Contains("high"))
                    return UniversalZoneDefinitions.Universal_ComHigh;
            }
            
            return null;
        }

        private int GetProbability(string zoneName, Setting? setting)
        {
            // IMPORTANT: Probability should affect spawn rates, not prefab creation
            // Always return 100 here so ALL building variants are created
            // The probability settings should be used elsewhere for spawn weighting
            return 100;
            
            /* Original probability logic - commented out as it causes missing assets
            if (setting == null) return 100;
            
            if (zoneName.Contains("NA Residential Low")) return setting.NALowRes;
            if (zoneName.Contains("EU Residential Low")) return setting.EULowRes;
            if (zoneName.Contains("NA Residential Medium Row")) return setting.NARowRes;
            if (zoneName.Contains("EU Residential Medium Row")) return setting.EURowRes;
            if (zoneName.Contains("NA Residential Medium")) return setting.NAMedRes;
            if (zoneName.Contains("EU Residential Medium")) return setting.EUMedRes;
            if (zoneName.Contains("NA Residential High")) return setting.NAHighRes;
            if (zoneName.Contains("EU Residential High")) return setting.EUHighRes;
            if (zoneName.Contains("NA Commercial Low")) return setting.NAComLow;
            if (zoneName.Contains("EU Commercial Low")) return setting.EUComLow;
            if (zoneName.Contains("NA Commercial High")) return setting.NAComHigh;
            if (zoneName.Contains("EU Commercial High")) return setting.EUComHigh;
            if (zoneName.Contains("NA Residential Mixed")) return setting.NAMixed;
            if (zoneName.Contains("EU Residential Mixed")) return setting.EUMixed;
            if (zoneName.Contains("Office")) return setting.Office;
            if (zoneName.Contains("Industrial")) return setting.Industrial;
            
            if (!zoneName.Contains("NA ") && !zoneName.Contains("EU "))
            {
                return setting.Other;
            }
            
            return 100;
            */
        }
    }
}
