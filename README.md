# Blazor Portfolio & BauToolKit

Dieses Repository umfasst die persönliche **Blazor Portfolio** Web-Applikation sowie das neue Projekt **BauToolKit** – ein plattformübergreifender Baustellen-Manager für Handwerks- und Bauunternehmen.

---

## BauToolKit – Baustellen-Manager

### 1. Projektziel und Zielgruppen

**BauToolKit** digitalisiert die Organisation von Baustellen, Materialflüssen, Arbeitszeiten und Fotodokumentationen zwischen Büro und Baustelle. Die Architektur basiert auf **.NET 8**, **Blazor Web App** und **.NET MAUI Blazor Hybrid**.

**Zielgruppen:**
- **Chefs / Bauleiter / Meister:** Haben die Gesamtübersicht im Büro oder auf dem Tablet über Kunden, Baustellen, Baufortschritt, Lagerbestand, Mitarbeiterzuweisungen und Rechnungen.
- **Mitarbeiter / Handwerker vor Ort:** Können über Smartphone oder Tablet ihre zugewiesenen Baustellen einsehen, Material entnehmen oder buchen, Arbeitszeiten erfassen und Fotos direkt vor Ort hochladen.

---

### 2. Geplante Rollen und Berechtigungen

- **Chef (`UserRole.Chef`):**
  - Kundenverwaltung (Erstellen, Bearbeiten, Deaktivieren)
  - Baustellenverwaltung & Überwachung des Baufortschritts
  - Zuweisen von Mitarbeitern zu Baustellen
  - Lagerverwaltung (Artikelbestand, Mindestmengen, Nachbestellungen)
  - Rechnungsabwicklung (Abschlagsrechnungen, Schlussrechnungen)
  - Einsicht in alle Zeiteinträge, Materialbuchungen und Bautagebücher/Fotos
  - 3D-Aufmaß und Projektplanung (in Ausbaustufe 2)

- **Mitarbeiter (`UserRole.Mitarbeiter`):**
  - Zugriff auf die eigenen, aktiv zugewiesenen Baustellen
  - Erfassung von Arbeitszeiten (Stunden, Tätigkeitsbeschreibung)
  - Materialbuchungen (Entnahme aus Lager / Rückgabe)
  - Upload von Baustellenfotos und Dokumentation vor Ort

*(Hinweis: Keine unsichere Eigenbau-Authentifizierung. Das MVP nutzt klare Domänenrollen und bereitet den Einsatz von ASP.NET Core Identity vor.)*

---

### 3. MVP-Funktionsumfang

Das vorliegende MVP-Fundament umfasst:
1. **Kunden- und Baustellenverwaltung:** Stammdatenverwaltung von Kunden und Baustellen inklusive Status (`Geplant`, `InArbeit`, `Pausiert`, `Abgeschlossen`).
2. **Mitarbeiter-Zuweisung:** Zuweisung von Mitarbeitern zu Baustellen mit Notizen und Deaktivierungsmöglichkeit.
3. **Lager- und Materialwirtschaft:** Erfassung von Lagerartikeln sowie Zu- und Abbuchungen (`Zugang`, `Verbrauch`, `Rueckgabe`) bezogen auf Baustellen.
4. **Arbeitszeiterfassung:** Erfassung von Arbeitsstunden mit Tätigkeitsnachweis und Stundensätzen.
5. **Fotodokumentation:** Upload (multipart/form-data), Verwaltung und Abruf von Baustellenfotos für Bautagebuch und Abnahmen.
6. **Rechnungsgrundlagen:** Domänenmodelle für Rechnungen und Rechnungspositionen (`Abschlagsrechnung`, `Schlussrechnung`, `Einzelforderung` mit Status `Entwurf`, `Gestellt`, `Bezahlt`, `Storniert`).
7. **Aufmaß-Fundament:** Datenstrukturen für Vermessungspunkte und Aufmaßprojekte als Vorbereitung für den 3D-Hausplaner.

---

### 4. Architektur und Solution-Struktur

Die Solution folgt den Prinzipien der Clean Architecture mit modularen Schichten:

```text
blazor-portfolio/
├── BauToolKit.sln                      # Haupt-Solution (enthält alle Projekte)
├── blazor-portfolio.csproj             # Bestehende Portfolio Web App
├── docs/
│   └── BauToolKit-Projektidee.txt      # Ursprüngliche Vision & fachlicher Kontext
├── src/
│   ├── BauToolKit.Domain/              # Domänenmodelle & Enums (POCOs, keine externen Abhängigkeiten)
│   ├── BauToolKit.Application/         # Schnittstellen, DTOs & Use-Case-Services
│   ├── BauToolKit.Infrastructure/      # In-Memory-Persistenz & lokaler Dateispeicher
│   ├── BauToolKit.Api/                 # ASP.NET Core Minimal API Backend
│   ├── BauToolKit.Web/                 # Blazor Web App Frontend (Büro / Desktop)
│   ├── BauToolKit.Mobile/              # Blazor Hybrid Komponentenbibliothek (wiederverwendbar)
│   └── BauToolKit.Mobile.Host/         # .NET MAUI Blazor Hybrid Host-App (Android, iOS, Windows)
└── tests/
    └── BauToolKit.UnitTests/           # Unit-Tests (xUnit, FluentAssertions, NSubstitute)
```

Die verbindlichen Anforderungen und die Roadmap für jedes der neun .NET-Projekte sind in [docs/Projektanforderungen.md](docs/Projektanforderungen.md) festgehalten. Projektänderungen sollen diese Roadmap einhalten und bei Bedarf dort nachgeführt werden.

---

### 5. Abgrenzung: 3D-Aufmaß / 3D-Hausplaner

In der ursprünglichen Projektidee ist ein integrierter 3D-Hausplaner vorgesehen, bei dem vor Ort aufgemessen wird und die Anwendung ein 3D-Modell erzeugt.
- **Aktueller Stand:** Die Domänenmodelle `MeasurementProject` und `SurveyPoint` mit Statusverwaltung (`Erfasst`, `InBearbeitung`, `Exportiert`) bilden das Aufmaß-Fundament. Zusätzlich bietet der Web-Frontend-Prototyp unter `/badplaner` eine isometrische Badplanung mit anpassbaren Raummaßen und platzierbaren Sanitärobjekten. Der Entwurf wird derzeit nur während der Sitzung gehalten und nicht gespeichert.
- **Spätere Ausbaustufe:** Persistente Planprojekte, maßstabsgetreue 3D-Visualisierung (z. B. via Three.js / WebGL oder CAD-Import) und Bluetooth-Messwerkzeuge (z. B. Laser-Distanzmessgeräte) bleiben eigenständige Erweiterungen.

---

### 6. Lokale Start-, Build- und Test-Anweisungen

#### Voraussetzungen
- .NET 8 SDK (oder höher)

#### Bauen der Solution
```bash
dotnet build BauToolKit.sln
```

#### Ausführen der Unit-Tests
```bash
dotnet test BauToolKit.sln
```

#### Backend API starten
```bash
dotnet run --project src/BauToolKit.Api
```
Die API stellt u. a. einen Status-Endpunkt unter `GET /api/status` sowie Endpunkte für Kunden, Baustellen, Zuweisungen, Lagerartikel, Arbeitszeiten und Fotodokumentation bereit.

#### Blazor Web App starten
```bash
dotnet run --project src/BauToolKit.Web
```

#### Ursprüngliches Portfolio starten
```bash
dotnet run --project blazor-portfolio.csproj
```

#### BauToolKit auf dem Smartphone testen

Es gibt zwei Wege, BauToolKit auf dem Smartphone auszuprobieren:

##### Option A: Sofort im mobilen Browser testen (ohne Installation)
1. Starte die Web App im lokalen Netzwerk:
   ```bash
   dotnet run --project src/BauToolKit.Web --urls "http://0.0.0.0:5000"
   ```
2. Ermittle die lokale IP-Adresse deines Entwicklungsrechners (z. B. `192.168.178.50`).
3. Verbinde dein Smartphone mit demselben WLAN-Netzwerk und rufe im mobilen Browser auf:
   ```text
   http://192.168.178.50:5000
   ```
   Die Blazor Web App ist responsiv optimiert und passt sich an Smartphone-Bildschirme an.

##### Option B: Als native App (.NET MAUI Blazor Hybrid) installieren
1. **Voraussetzung:** Installiere die .NET MAUI Workloads auf deinem Entwicklungs-PC:
   ```bash
   dotnet workload install maui
   # oder gezielt für Android:
   dotnet workload install maui-android
   ```
2. **Android Smartphone vorbereiten:**
   - Aktiviere die *Entwickleroptionen* und *USB-Debugging* in den Android-Einstellungen.
   - Schließe das Smartphone per USB-Kabel an den PC an.
3. **App direkt auf dem angeschlossenen Smartphone oder Emulator ausführen:**
   ```bash
   dotnet build src/BauToolKit.Mobile.Host/BauToolKit.Mobile.Host.csproj -t:Run -f net8.0-android -p:BuildingForMaui=true -p:AndroidOnlyBuild=true
   ```
   *(Alternativ in Visual Studio: `BauToolKit.Mobile.Host` als Startprojekt festlegen, Zielplattform `net8.0-android` und das Smartphone als Zielgerät auswählen und auf „Starten“ drücken).*
4. **Standalone APK zum manuellen Installieren bauen:**
   ```bash
   dotnet publish src/BauToolKit.Mobile.Host/BauToolKit.Mobile.Host.csproj -f net8.0-android -c Release -p:BuildingForMaui=true -p:AndroidOnlyBuild=true
   ```
   Die erzeugte `.apk`-Datei befindet sich in `src/BauToolKit.Mobile.Host/bin/Release/net8.0-android/publish/` und kann direkt auf das Smartphone kopiert und installiert werden.

---

### 7. Coding Standards und Konventionen

Entsprechend `.github/copilot-instructions.md`:
- Target Framework: **.NET 8**
- **File-scoped namespaces** in allen C#-Dateien
- **Primary Constructors** für Dependency Injection
- Strenge Nullable-Prüfung (**`#nullable enable`**)
- Explizite Typen (`var` nur bei eindeutig erkennbarem Typ auf der rechten Seite)
- Asynchrone Signaturen mit **`CancellationToken`** für alle I/O-Methoden
- Test-Frameworks: **xUnit**, **FluentAssertions**, **NSubstitute**

---

### 8. Fachlicher Kontext für spätere Agents

Die ungekürzte ursprüngliche Projektanfrage des Erstellers ist dokumentiert unter:
`docs/BauToolKit-Projektidee.txt` (Stand: 1. Oktober 2026).
