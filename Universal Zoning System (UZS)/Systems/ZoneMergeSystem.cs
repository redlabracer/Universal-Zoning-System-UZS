using Game;
using Game.Common;
using Game.Prefabs;
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
        private const int BATCH_SIZE = 20; // Reduced from 200 to 20 to prevent freezing
        private int m_ClonedCount = 0;
        private HashSet<string> m_ExistingPrefabNames = new HashSet<string>();

        // Optimization: Cache zone info
        private struct ZoneMergeInfo
        {
            public string TargetUniversalZone;
            public int Probability;
        }
        private Dictionary<Entity, ZoneMergeInfo> m_ZoneMergeInfos = new Dictionary<Entity, ZoneMergeInfo>();

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
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
            if (m_State == 0)
            {
                // Wait for prefabs to be loaded
                var query = GetEntityQuery(ComponentType.ReadOnly<PrefabData>());
                if (query.CalculateEntityCount() > 0)
                {
                    CreateUniversalZones();
                    m_State = 1;
                }
            }
            else if (m_State == 1)
            {
                if (CheckUniversalZonesReady())
                {
                    StartMerge();
                    // Run the entire merge process synchronously to avoid gameplay lag
                    ProcessMergeBatch();
                    m_State = 3; // Done
                    Enabled = false; // Disable system
                }
            }
        }

        private void CreateUniversalZones()
        {
            // Build lookup of existing zones
            var zoneLookup = new Dictionary<string, Entity>();
            
            // FIX: Query PrefabData instead of ZonePrefab component
            // ZonePrefab is a managed type, not a component data struct, so we must query PrefabData
            var zoneQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>());
            var zoneEntities = zoneQuery.ToEntityArray(Allocator.Temp);
            
            foreach (var entity in zoneEntities)
            {
                var prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
                if (prefab != null)
                {
                    if (!zoneLookup.ContainsKey(prefab.name))
                    {
                        zoneLookup.Add(prefab.name, entity);
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

            foreach (var kvp in zonesToCreate)
            {
                Entity templateEntity = Entity.Null;
                if (zoneLookup.TryGetValue(kvp.Value, out templateEntity))
                {
                    var sourcePrefab = m_PrefabSystem.GetPrefab<ZonePrefab>(templateEntity);
                    if (sourcePrefab != null)
                    {
                        // Create the new Universal Zone based on the NA template
                        var newPrefab = Object.Instantiate(sourcePrefab);
                        newPrefab.name = kvp.Key;
                        
                        // Customize the UI to show it's Universal
                        // (In a real mod, you'd change the icon or color here)
                        
                        m_PrefabSystem.AddPrefab(newPrefab);
                        m_CreatedPrefabs[kvp.Key] = newPrefab;
                        UnityEngine.Debug.Log($"UZS: Created universal zone '{kvp.Key}' from '{kvp.Value}'");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"UZS: Could not find template zone '{kvp.Value}' for '{kvp.Key}'");
                }
            }
        }

        private bool CheckUniversalZonesReady()
        {
            if (m_CreatedPrefabs.Count == 0) return false;
            bool allReady = true;
            foreach(var kvp in m_CreatedPrefabs)
            {
                Entity e = m_PrefabSystem.GetEntity(kvp.Value);
                if (e == Entity.Null) allReady = false;
                else m_UniversalZoneEntities[kvp.Key] = e;
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

            while (m_ProcessIndex < m_EntitiesToProcess.Length)
            {
                Entity entity = m_EntitiesToProcess[m_ProcessIndex];
                m_ProcessIndex++;
                
                // Get the managed prefab associated with this entity
                var prefab = m_PrefabSystem.GetPrefab<BuildingPrefab>(entity);
                if (prefab == null) continue;

                // CRITICAL FIX: Skip if already a Universal clone to prevent exponential growth
                if (prefab.name.StartsWith("Universal_")) continue;

                // CRITICAL FIX: Skip if the target Universal prefab already exists
                if (m_ExistingPrefabNames.Contains("Universal_" + prefab.name)) continue;

                var spawnableData = EntityManager.GetComponentData<SpawnableBuildingData>(entity);
                Entity zoneEntity = spawnableData.m_ZonePrefab;
                
                if (zoneEntity != Entity.Null)
                {
                    // Optimization: Use cached zone info
                    if (m_ZoneMergeInfos.TryGetValue(zoneEntity, out ZoneMergeInfo info))
                    {
                        if (m_CreatedPrefabs.TryGetValue(info.TargetUniversalZone, out ZonePrefab universalZonePrefab))
                        {
                            // Check probability settings
                            if (UnityEngine.Random.Range(0, 100) >= info.Probability)
                            {
                                continue;
                            }

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
                    }
                }
            }

            double elapsed = (System.Diagnostics.Stopwatch.GetTimestamp() - startTime) / (double)System.Diagnostics.Stopwatch.Frequency;
            UnityEngine.Debug.Log($"UZS: Finished cloning. Total cloned: {m_ClonedCount}. Time taken: {elapsed:F2} seconds.");
            
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
                        // UnityEngine.Debug.Log($"UZS: Mapped zone '{zoneName}' to '{target}'");
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
        }
    }
}
