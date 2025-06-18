# Projektidee Structurio

# Projektidee

**Name: Structurio**

**Structurio**– Traditionelle Projektverwaltung mit moderner Effizienz

**Team:** Valentin und Max

---

# Mindestanforderungen

- **Login-Seite**: Rollenbasierte Anmeldung (Manager, Mitarbeiter, Buchhalter).
- **Projekterstellung**: Manager können Projekte anlegen und Teammitglieder zuweisen.
- **Projektverwaltung**:
    - Aufgabenzuweisung
    - Arbeitszeiterfassung
- **Buchhaltung**:
    - Budgetierung pro Projekt
    - Erfassung von Abrechnungen und Ausgaben
- **Benutzerrollen**:
    - Unterschiedliche Rechte für Manager, Mitarbeiter, Buchhalter
- **Technische Anforderungen**:
    - MariaDB-Datenbank, REST-API mit Swagger-Dokumentation
    - GitHub-Projektmanagement (Kanban)
    - Unit-Tests, Logging
    - Grafisches User-Interface mit mindestens 3 Fenstern

---

# Muss-Features (Pflicht)

✅ Login & Rollenmanagement

✅ Projekte erstellen und verwalten

✅ Aufgaben und Zeiterfassung pro Mitarbeiter

✅ Budgetverwaltung & Abrechnungen (Projektmanager)

✅ API-Dokumentation, Unit Tests, Logging

---

# Nice-to-Have-Features (Erweiterungen)

✨ Kommentarfunktion bei Aufgaben

✨ Erinnerungen für fällige Aufgaben

✨ Cloud-Datenbank-Anbindung

✨ Exportfunktion für Berichte

✨ Diagrammübersicht für Projektfortschritt und Budget

✨ Urlaub beantragen / Freistellung für Fortbildungen beantragen

✨ Mitarbeiter können sich ihre Stunden ausdrucken(Zeiterfassung visualisieren (wie viele Überstunden…) und als PDF bereitstellen mit Analyse (und auch als Seite in der App))

✨ Mitarbeiter können ausgaben für das Projekt verrechnen (beantragen und wird dann von Projektmanager Genehmigt/Abgelehnt)

---

# Grobe Umsetzung

- **Frontend**:
    
    C# WPF-Anwendung mit mindestens drei Hauptfenstern:
    
    1. Login-Seite
    2. Dashboard für Projekte und Aufgaben
    3. Buchhaltungsbereich (Budgetübersicht und Abrechnungen) für den Projektmanager
        1. Ressourcen festlegen
        2. Abrechnen z.B. Monatslohn oder Computer
- **Backend**:
    
    REST-API in C# oder Python (z. B. Flask/FastAPI), angebunden an eine MariaDB-Datenbank.
    
- **Projektdatenbank**:
    
    Tabellen für Benutzer, Projekte, Aufgaben, Zeitstempel, Budgets und Abrechnungen.
    
- **Projektplanung**:
    
    GitHub-Projekt-Board zur Aufgabenverteilung, Dokumentation der Arbeitsschritte in Markdown.