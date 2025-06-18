# Lastenheft Structurio
---

# Allgemeine Mindestanforderungen

## Organisatorisch

* 2er Team
* Arbeit teils im Unterricht, teils zuhause
* Abgabetermin: spätestens 18.06.2025, 23:59 Uhr

## Technische Anforderungen

### POS

* Verwendung von Git
* Projektplanung über GitHub (inkl. Kanban-Board)
* Klassendiagramme vor Beginn der Entwicklung
* Grafische Anwendung mit mindestens 3 Fenstern
* Einsatz von Vererbung und abstrakten Klassen
* Nutzung von Interfaces
* Logging
* API-Dokumentation
* Unit Tests

### DBI

* Verwendung von mindestens 3 Tabellen in 3. Normalform
* Abfragen mit SELECT, JOIN, Aggregation
* Lese- und Schreibzugriffe
* Zwei Benutzerrollen mit Rechten (Admin, User)
* Einsatz einer MariaDB-Datenbank (kein SQLite)
* REST-API (C# oder Python)
* Swagger-Dokumentation + JSON-Schema
* Logging
* Unit Tests

---

# Projektplanung Structurio

## Grobe Idee

Eine WPF-Anwendung zur Verwaltung von Projekten:
Benutzer können Projekte anlegen, Aufgaben verwalten (Kanban), Zeit erfassen, Ausgaben dokumentieren und Projektbudgets überwachen. Dokumente lassen sich verwalten und ein einfaches Login-System ist integriert.

---

## Mindestanforderungen Structurio

| Feature                                 | Erfüllt
| --------------------------------------- | ---------
| Login-System                            | Ja
| Projekte anlegen                        | Ja
| Projektübersicht                        | Ja
| Aufgabenverwaltung (Tickets)            | Ja
| Zeiterfassung                           | Ja
| Budget-/Kostenverwaltung                | Ja
| REST-API + Swagger                      | Ja
| Logging                                 | Ja
| API-Dokumentation                       | Ja
| GUI mit ≥ 3 Fenstern                    | Ja
| Git + GitHub-Kanban                     | Ja
| Unit Tests                              | Ja

---

## Nice-to-Haves Structurio

| Feature                                            | Erfüllt
| -------------------------------------------------- | ---------
| Cloud-Datenbank (z. B. Neon-Tech)                   | Ja
| PDF-View und Export (z. B. Stunden oder Abrechnung) | Ja
| Diagrammübersicht (z. B. Projektbudget)            | Ja
| Abrechnungen Grafisch Dargestellt                   | Ja


# Abreitsaufteilung

| Teammitglied | Aufgabenbereich |
| ------------ | ---------------- |
| Valentin | Projekte, Kanban, Dateiensystem, Projekteverwaltung , (Frontend und Backend) |
| Maximilian | Timestamp, Kostenverwaltung mit Eintrag und Visualisierung, PDF Viewer für Rechnungen, (Frontend und Backend) |