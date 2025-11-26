# Troubleshooting: Zone-Probleme nach Safe Uninstall

## Problem: Gebäude haben Texturen, aber rote Fehler-Icons (nicht in Zone)

### Ursache
Die `SpawnableBuildingData.m_ZonePrefab` Komponente der Gebäude zeigt noch auf die Universal-Zonen, die nicht mehr existieren.

### Lösung (v1.1+)
Das wurde in Version 1.1 behoben. Das Safe Uninstall System aktualisiert jetzt:
1. ? `PrefabRef.m_Prefab` ? Original-Gebäude-Prefab
2. ? `SpawnableBuildingData.m_ZonePrefab` ? Original-Zonen-Entity

### Wie man prüft, ob es funktioniert hat

1. **Konsole überprüfen (F7 im Spiel)**
   Suchen Sie nach diesen Log-Einträgen:
   ```
   UZS: Safe uninstall process started!
   UZS: Built original zone lookup with 8 entries.
   UZS: Found XXX buildings to check.
   UZS: Converted building 'BuildingName' from zone 'Universal_ResLow' to original zone.
   UZS: Converted XXX buildings from Universal to original prefabs and zones.
   UZS: Safe uninstall complete!
   ```

2. **Visuelle Überprüfung**
   - Gebäude sollten Texturen haben ?
   - Keine roten Fehler-Icons mehr ?
   - Gebäude bleiben stehen ?

### Wenn es immer noch nicht funktioniert

1. **Prüfen Sie, ob die Mod aktiv ist**
   - Die Mod MUSS aktiviert sein, um Safe Uninstall zu verwenden
   - Laden Sie den Spielstand mit aktiver Mod

2. **Prüfen Sie die Logs**
   - Öffnen Sie die Unity-Konsole (F7)
   - Suchen Sie nach "UZS:" Nachrichten
   - Prüfen Sie auf Fehler oder Warnungen

3. **Manuelle Prüfung**
   ```
   UZS: Mapped Universal_ResLow -> NA Residential Low (Entity: ...)
   ```
   Wenn diese Einträge fehlen, konnte das System die Original-Zonen nicht finden.

4. **Neustart erforderlich?**
   - Speichern Sie nach dem Safe Uninstall
   - Laden Sie den Spielstand neu
   - Dann erst die Mod deaktivieren

## Debug-Informationen für Entwickler

### Wichtige Komponenten
- `PrefabRef`: Referenz zum Gebäude-Prefab
- `SpawnableBuildingData`: Enthält `m_ZonePrefab` (Zone-Entity-Referenz)
- `Building`: Hauptkomponente für Gebäude-Entities

### Code-Flow
```
1. BuildOriginalZoneLookup()
   ? Erstellt Dictionary: Universal_ResLow ? NA Residential Low Entity

2. ConvertUniversalBuildings()
   ? Für jedes Gebäude mit "Universal_" Prefab:
     a. Finde Original-Prefab (ohne "Universal_" Prefix)
     b. Update PrefabRef.m_Prefab
     c. Update SpawnableBuildingData.m_ZonePrefab
```

### Häufige Fehlerquellen
1. ? `EntityManager.HasComponent<SpawnableBuildingData>()` gibt false zurück
   - Gebäude ist kein Zonen-Gebäude
   - Sollte nicht passieren bei "Universal_" Prefabs

2. ? `m_OriginalZones[universalZoneName]` existiert nicht
   - Original-Zone wurde nicht gefunden
   - Prüfen Sie Zone-Namen-Mapping

3. ? `FindPrefabByName()` gibt Entity.Null zurück
   - Original-Gebäude-Prefab existiert nicht mehr
   - Prefab wurde durch Update entfernt/umbenannt

## Testen

### Test-Szenario
1. Erstellen Sie einen neuen Spielstand
2. Platzieren Sie Universal-Zonen
3. Bauen Sie Gebäude
4. Aktivieren Sie Safe Uninstall
5. Speichern und neu laden
6. Deaktivieren Sie die Mod
7. Laden Sie den Spielstand

**Erwartetes Ergebnis:**
- Alle Gebäude haben Texturen
- Keine Fehler-Icons
- Gebäude bleiben stehen
- Keine Konsolen-Fehler

## Version History

### v1.0 (Original)
- ? Nur PrefabRef aktualisiert
- ? Zone-Referenzen nicht aktualisiert
- ? Gebäude hatten "nicht in Zone" Fehler

### v1.1 (Current)
- ? PrefabRef aktualisiert
- ? SpawnableBuildingData.m_ZonePrefab aktualisiert
- ? Gebäude funktionieren korrekt
- ? Keine Fehler-Icons mehr
