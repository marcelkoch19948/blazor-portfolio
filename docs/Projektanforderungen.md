# Projektanforderungen und Roadmap

Dieses Dokument ist die verbindliche, projektbezogene Anforderungsspezifikation für alle neun .NET-Projekte der Solution. Es wird bei Änderungen an Umfang, Architektur oder Roadmap gemeinsam mit dem betroffenen Projekt aktualisiert. Bestehende MVP-Funktionen sind der aktuelle Stand; spätere Ausbaustufen werden erst umgesetzt, wenn ihre Anforderungen ergänzt und abgestimmt sind.

## Gemeinsame technische Anforderungen

- Ziel-Framework ist .NET 8; .NET 9 ist nur nach abgestimmter Aktualisierung der Solution vorgesehen.
- C#-Dateien verwenden dateibezogene Namespaces und aktivierte Nullable-Referenztypen.
- Dependency-Injection-Konstruktoren bevorzugen Primary Constructors.
- I/O-bound asynchrone Methoden akzeptieren und propagieren einen `CancellationToken`.
- Unit-Tests verwenden xUnit, NSubstitute und FluentAssertions.
- Abhängigkeiten folgen der Clean Architecture: Domain bleibt unabhängig; Application hängt von Domain ab; Infrastructure implementiert Application-Abstraktionen; API und UIs sind Composition Roots.

## `blazor-portfolio.csproj` — Portfolio

**Ziel und Anforderungen:** Die bestehende Blazor-Portfolio-Webanwendung bleibt als eigenständiges Produkt erhalten. Sie nutzt Blazor Server Interaktivität, bewahrt ihre vorhandenen Routen und Assets und muss auf Desktop- und Mobilbildschirmen bedienbar bleiben.

**Roadmap:**
1. Bestehende Seiten und Navigation zuverlässig erhalten.
2. Portfolio-Inhalte, responsive Bedienbarkeit und barrierearme Darstellung gezielt weiterentwickeln.

## `src/BauToolKit.Domain/BauToolKit.Domain.csproj` — Domain

**Ziel und Anforderungen:** Fachmodelle und Enums für Kunden, Baustellen, Mitarbeiterzuweisungen, Lager und Materialbuchungen, Arbeitszeiten, Fotodokumentation, Rechnungen und Aufmaß bilden die fachliche Grundlage. Die Domain enthält keine Infrastruktur- oder UI-Abhängigkeiten.

**Roadmap:**
1. Fachliche Invarianten und Statusübergänge der vorhandenen Modelle konsistent halten.
2. Neue BauToolKit-Fachanforderungen zuerst domänenseitig spezifizieren und durch isolierte Unit-Tests absichern.

## `src/BauToolKit.Application/BauToolKit.Application.csproj` — Application

**Ziel und Anforderungen:** Use Cases, DTOs sowie Service- und Repository-Schnittstellen kapseln Geschäftsabläufe. Die Schicht hängt nur von der Domain und erforderlichen Abstraktionen ab; asynchrone Operationen propagieren Cancellation Tokens.

**Roadmap:**
1. Bestehende Use Cases für Baustellen, Kunden, Zuweisungen, Lager, Material, Zeiten, Fotos und Rechnungen beibehalten.
2. Fehlende Validierung und fachliche Fehlerfälle der Use Cases festlegen und testen.
3. Rollenbezogene Zugriffspolitiken erst mit einem abgestimmten Authentifizierungsansatz ergänzen; keine Eigenbau-Authentifizierung einführen.

## `src/BauToolKit.Infrastructure/BauToolKit.Infrastructure.csproj` — Infrastructure

**Ziel und Anforderungen:** Die Schicht implementiert Application-Abstraktionen durch In-Memory-Repositories und lokalen Dateispeicher. Infrastrukturdetails bleiben hinter den Schnittstellen verborgen.

**Roadmap:**
1. Verhalten und Cancellation-Unterstützung der Repository- und Dateispeicherimplementierungen testen.
2. Dauerhafte Datenbank- und Dateispeicherlösungen als gesonderte Ausbaustufe spezifizieren, bevor In-Memory-Speicher ersetzt werden.

## `src/BauToolKit.Api/BauToolKit.Api.csproj` — API

**Ziel und Anforderungen:** Die ASP.NET-Core-Minimal-API stellt die Application-Use-Cases für Status, Kunden, Baustellen, Zuweisungen, Lager und Material, Arbeitszeiten, Fotos sowie Rechnungen bereit. HTTP-Aufrufe berücksichtigen Request-Cancellation.

**Roadmap:**
1. Vorhandene Endpunkte und ihre Erfolgs-, Validierungs- und Fehlerantworten stabilisieren.
2. API-Verträge und End-to-End-Verhalten testen.
3. Authentifizierung und Autorisierung ausschließlich auf Basis eines abgestimmten Identity-Konzepts ergänzen.

## `src/BauToolKit.Web/BauToolKit.Web.csproj` — Web

**Ziel und Anforderungen:** Die Blazor-Web-App unterstützt Büro- und Desktop-Arbeitsabläufe für Baustellen, Mitarbeitende, Lager, Fotodokumentation, Zeiterfassung und Rechnungen. Der Badplaner-Prototyp bleibt als nicht persistente Funktion klar abgegrenzt.

**Roadmap:**
1. Vorhandene Seiten bedienbar und mit den Application-Funktionen konsistent halten.
2. Responsive Nutzung für mobile Browser erhalten.
3. Persistenz oder weitergehende 3D-Funktionen für den Badplaner nur als separate, spezifizierte Erweiterung umsetzen.

## `src/BauToolKit.Mobile/BauToolKit.Mobile.csproj` — Mobile-Komponenten

**Ziel und Anforderungen:** Wiederverwendbare Blazor-Komponenten unterstützen mobile Baustellenabläufe, insbesondere Baustellenübersicht, Materialbuchung und Arbeitszeiterfassung. Die Komponentenbibliothek bleibt unabhängig von plattformspezifischen Host-Details.

**Roadmap:**
1. Formulare und Karten mit den Application-Verträgen konsistent halten und Eingaben verständlich validieren.
2. Offline-Nutzung und Synchronisierung erst nach Definition der Datenkonflikt- und Persistenzanforderungen ergänzen.

## `src/BauToolKit.Mobile.Host/BauToolKit.Mobile.Host.csproj` — MAUI Host

**Ziel und Anforderungen:** Der .NET-MAUI-Blazor-Hybrid-Host bindet die wiederverwendbaren Mobile-Komponenten für Android, iOS, Mac Catalyst und Windows ein. Plattformabhängige Funktionalität gehört in den Host, nicht in die gemeinsame Komponentenbibliothek.

**Roadmap:**
1. Start und Kernabläufe auf den unterstützten Plattformen verifizieren.
2. Geräteberechtigungen und native Funktionen einzeln spezifizieren und plattformbezogen testen, bevor sie aktiviert werden.

## `tests/BauToolKit.UnitTests/BauToolKit.UnitTests.csproj` — Unit-Tests

**Ziel und Anforderungen:** xUnit-Tests mit FluentAssertions und NSubstitute schützen Domänenregeln und Application-Use-Cases. Tests bleiben reproduzierbar und benötigen keine externen Dienste.

**Roadmap:**
1. Bestehende Domain- und Service-Tests für die MVP-Funktionen erhalten.
2. Validierungs-, Grenz- und Fehlerfälle ergänzen, wenn die zugehörigen Anforderungen festgelegt werden.
3. Neue fachliche und Application-Funktionen gemeinsam mit ihren Unit-Tests bereitstellen.
