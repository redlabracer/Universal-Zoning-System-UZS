# Safe Uninstall Guide for Universal Zoning System

## Das Problem
Wenn Sie die Universal Zoning System Mod auf einem Spielstand deaktivieren, der sie verwendet hat, werden alle Gebäude, die auf Universal-Zonen platziert wurden:
- Ihre Texturen verlieren (grau/leer erscheinen)
- Für den Abriss markiert (rote Fehler-Icons)
- Schließlich verschwinden

Dies geschieht, weil das Spiel die Zone-Prefabs nicht mehr finden kann, mit denen diese Gebäude verknüpft waren.

## Die Lösung

### Option 1: Mod wieder aktivieren (Sofort-Fix)
1. Gehen Sie zurück zum Content Manager
2. Aktivieren Sie die "Universal Zoning System" Mod wieder
3. Laden Sie Ihren Spielstand
4. Ihre Gebäude sollten sich wieder korrekt darstellen

### Option 2: Safe Uninstall Funktion verwenden (NEU - EMPFOHLEN)

Ich habe gerade eine "Safe Uninstall" Funktion zur Mod hinzugefügt. So verwenden Sie sie:

1. **Aktivieren Sie die Mod zuerst wieder** (falls Sie sie deaktiviert haben)
2. Laden Sie Ihren Spielstand mit aktiver Mod
3. Öffnen Sie das **Optionen-Menü**
4. Navigieren Sie zu den **Universal Zoning System** Einstellungen
5. Klicken Sie auf den **"Safe Uninstall"** Tab
6. Klicken Sie auf den **"Prepare Save for Mod Removal"** Button
7. Warten Sie, bis der Prozess abgeschlossen ist (prüfen Sie die Logs)
8. Speichern Sie Ihr Spiel
9. Jetzt können Sie die Mod sicher deaktivieren

### Was macht Safe Uninstall?
- Scannt alle Gebäude in Ihrer Stadt
- Findet Gebäude, die Universal-Zone-Prefabs verwenden
- Konvertiert sie zurück zu ihren ursprünglichen Nord-Amerikanischen Zone-Prefabs
- **Aktualisiert die Zone-Referenzen der Gebäude** (verhindert "nicht in Zone" Fehler)
- Loggt den Fortschritt in der Unity-Konsole

**Wichtige Hinweise:**
- Dieser Prozess **kann nicht rückgängig gemacht werden**
- Gebäude werden zu NA (Nord-Amerikanischen) Zonen konvertiert
- EU-Stil Gebäude bleiben erhalten, sind aber mit NA-Zonen verknüpft
- Erstellen Sie ein Backup Ihres Spielstands, bevor Sie diese Funktion verwenden

## Technische Details

### Was wurde geändert (v1.1)
1. **Setting.cs**: "Safe Uninstall" Button in den Mod-Einstellungen hinzugefügt
2. **SafeUninstallSystem.cs**: Neues System, das:
   - Universal-Gebäude zurück zu Original-Prefabs konvertiert
   - **SpawnableBuildingData** Zone-Referenzen aktualisiert (behebt "nicht in Zone" Problem)
   - Zone-Entity-Verknüpfungen repariert
3. **Mod.cs**: Registriert das neue Uninstall-System
4. **LocaleEN.cs**: Lokalisierung für den Uninstall-Button hinzugefügt

### Wie es funktioniert
Das System:
1. Baut eine Lookup-Tabelle der Original-Zonen-Entities (NA Residential Low, etc.)
2. Fragt alle Gebäude in der Welt ab
3. Findet Gebäude mit "Universal_" Prefab-Namen
4. Ersetzt ihre PrefabRef mit dem Original nicht-Universal-Prefab
5. **Aktualisiert SpawnableBuildingData.m_ZonePrefab** von Universal-Zone zu Original-Zone
6. Loggt die Anzahl der Konvertierungen

### Warum passiert das?
Cities: Skylines 2 Mods können neue Prefabs erstellen (Zonentypen, Gebäude, etc.), die in Ihrem Spielstand gespeichert werden. Wenn Sie die Mod entfernen:
- Die Prefab-Definitionen sind weg
- Der Spielstand referenziert diese Prefabs noch
- Das Spiel kann sie nicht finden, was Fehler verursacht

Die Safe Uninstall Funktion ist darauf ausgelegt, diese Referenzen vor dem Entfernen zu bereinigen.

## Aktueller Fix (v1.1)
Die neue Version behebt auch das "nicht in Zone" Problem:
- ? Texturen bleiben erhalten
- ? Zone-Referenzen werden korrekt aktualisiert
- ? Keine Abriss-Warnungen mehr
- ? Gebäude bleiben stehen

## Safe Uninstall Schritte

### 1. Im Spiel (Vor dem Deaktivieren der Mod)

1. **Laden Sie Ihr Spiel** mit aktivierter Universal Zoning System Mod
2. **Öffnen Sie das Optionen-Menü** (drücken Sie ESC oder klicken Sie auf das Zahnrad-Symbol)
3. Suchen Sie den **"Safe Uninstall UZS" Button** im Allgemein-Bereich
4. **Klicken Sie auf den Button** - Sie sehen die Nachricht: *"Safe uninstall initiated! Check Player.log for progress."*
5. **Warten Sie 5-10 Sekunden** bis der Konvertierungsprozess abgeschlossen ist
6. **Überprüfen Sie die Logs** (siehe unten), um die Fertigstellung zu bestätigen:
   ```
   UZS: Safe uninstall process started!
   UZS: Built original zone lookup with X entries.
   UZS: Protected all buildings from despawning.
   UZS: Converted X buildings from Universal to original prefabs.
   UZS: Buildings are now protected from despawning. You can safely disable the mod.
   ```

### 2. Speichern Sie Ihr Spiel

Nachdem die Konvertierung abgeschlossen ist:

1. **Speichern Sie Ihr Spiel** (drücken Sie Ctrl+S oder verwenden Sie das Menü)
2. **Warten Sie, bis das Speichern vollständig abgeschlossen ist**
3. **Gehen Sie zurück zum Hauptmenü**

### 3. Deaktivieren Sie die Mod

1. **Gehen Sie zum Paradox Launcher** oder **zum Mod-Manager im Spiel**
2. **Deaktivieren Sie** "Universal Zoning System (UZS)"
3. **Starten Sie das Spiel neu**
4. **Laden Sie Ihr Spiel** - alle Gebäude sollten intakt bleiben!

## ?? Überprüfung

Nach dem Neuladen ohne die Mod:

- **Sollten alle Gebäude noch da sein**
- **Sollten keine Gebäude verschwinden**
- **Sollten Zonen normal funktionieren** mit den Original-Prefabs
- **Überprüfen Sie, ob die geschützten Gebäude verbleiben** (sie haben das Marker nicht mehr, sollten aber da sein)

## ?? Was Wird Konvertiert

Der Safe Uninstall Prozess konvertiert:

| Universelle Zone | ? | Original Zone |
|----------------|---|---------------|
| Universal_ResLow | ? | NA Residential Low |
| Universal_ResRow | ? | NA Residential Medium Row |
| Universal_ResMed | ? | NA Residential Medium |
| Universal_ResHigh | ? | NA Residential High |
| Universal_ResLowRent | ? | Residential LowRent |
| Universal_ComLow | ? | NA Commercial Low |
| Universal_ComHigh | ? | NA Commercial High |
| Universal_Mixed | ? | NA Residential Mixed |

## ?? Wenn Gebäude Immer Noch Verschwinden

Wenn Sie feststellen, dass Gebäude verschwinden, selbst nachdem Sie die sichere Deinstallation durchgeführt haben:

1. **Überprüfen Sie die Logs** (`%LocalAppData%Low\Colossal Order\Cities Skylines II\Player.log`)
2. **Suchen Sie nach Fehlermeldungen** von UZS
3. **Bestätigen Sie, dass die Konvertierung abgeschlossen ist** (suchen Sie nach der Nachricht "Safe uninstall complete!")
4. **Stellen Sie sicher, dass Sie nach der Konvertierung gespeichert haben**
5. **Melden Sie das Problem** mit Ihrer Player.log Datei

## ?? Bekannte Einschränkungen

- **Sie müssen die sichere Deinstallation vor dem Deaktivieren der Mod durchführen**
- **Der Prozess muss abgeschlossen sein** (nicht während der Konvertierung beenden)
- **Gebäude mit benutzerdefinierten Komponenten** werden möglicherweise nicht perfekt konvertiert
- **Zonenthemen** werden möglicherweise auf die Standardeinstellungen zurückgesetzt

## ?? Technische Details

Der sichere Deinstallationsprozess:

1. **Fragt alle Zone-Prefab-Entities ab** und erstellt eine Nachschlagetabelle
2. **Schützt ALLE Gebäude** vor dem Verschwinden, indem ein Markierungsbestandteil hinzugefügt wird
3. **Findet alle Gebäude** mit Universal-Prefabs
4. **Konvertiert die Prefab-Referenzen** von `Universal_*` zu Original-Prefabs
5. **Entfernt die Schutzmarkierungen** nach ein paar Frames, um normales Gameplay zu ermöglichen

Das `BuildingProtectionSystem` läuft kontinuierlich und:
- Überwacht die `Deleted`-Tags bei geschützten Gebäuden
- Entfernt automatisch die `Deleted`-Tags, um das Verschwinden zu verhindern
- Protokolliert Warnungen, wenn der Schutz ausgelöst wird

## ?? Protokollnachrichten

Erwartete Protokollnachrichten während der sicheren Deinstallation:

```
UZS: Safe uninstall process started!
UZS: Mapped Universal_ResLow -> NA Residential Low (Entity: X)
... [mehr Zuordnungen] ...
UZS: Built original zone lookup with 8 entries.
UZS: Protected all buildings from despawning.
UZS: Found X protected buildings to check.
UZS: Converted building 'BuildingName' from Universal prefab to original prefab.
... [mehr Konvertierungen] ...
UZS: Converted X buildings from Universal to original prefabs.
UZS: Removed protection from all buildings.
UZS: Safe uninstall complete! Converted X buildings back to original zones.
UZS: Buildings are now protected from despawning. You can safely disable the mod.
```

Wenn Sie "Preventing despawn of X protected buildings!" sehen - bedeutet das, dass das Schutzsystem funktioniert!

## ?? Tipps

- **Führen Sie die sichere Deinstallation zuerst in einem separaten Spielstand durch**, um es zu testen
- **Erstellen Sie einen Backup-Save**, bevor Sie die sichere Deinstallation durchführen (kopieren Sie die Spieldatei)
- **Beenden Sie die Anwendung nicht während der Konvertierung** - warten Sie auf die Abschlussnachricht
- **Überprüfen Sie die Protokolle nach jedem Schritt**, um den Fortschritt zu überprüfen
- **Melden Sie Probleme mit Ihrer Player.log Datei im Anhang**

## ?? Notfall-Wiederherstellung

Wenn Sie die Mod deaktiviert haben, ohne die sichere Deinstallation durchzuführen:

1. **Aktivieren Sie die Mod wieder**
2. **Laden Sie Ihr Spiel**
3. **Führen Sie die sichere Deinstallation wie oben beschrieben durch**
4. **Speichern Sie das Spiel**
5. **Deaktivieren Sie dann die Mod erneut**

---

**Denken Sie daran**: Der sichere Deinstallationsprozess ist **essenziell**, um Datenverlust zu verhindern. Führen Sie ihn immer aus, bevor Sie das Universal Zoning System deaktivieren!
