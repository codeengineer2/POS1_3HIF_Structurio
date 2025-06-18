using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// @file Abrechnung_JSON.cs
/// @brief Datenmodell für Abrechnungen.

namespace Structurio
{

    /// @class Abrechnung_JSON
    /// @brief Repräsentiert eine einzelne Abrechnungseinheit zur (De)Serialisierung.
    public class Abrechnung_JSON
    {

        /// @brief Die eindeutige ID der Abrechnung (Primary Key in der Datenbank).
        public int Aid { get; set; }

        /// @brief Die Benutzer-ID, zu der diese Abrechnung gehört.
        public int Uid { get; set; }

        /// @brief Die Projekt-ID, der diese Abrechnung zugeordnet ist.
        public int Pid { get; set; }

        /// @brief Der Name oder Titel der Abrechnung (z. B. Beschreibung der Ausgabe).
        public string Name { get; set; }

        /// @brief Das Datum, an dem die Ausgabe entstanden ist.
        public DateTime Date { get; set; }

        /// @brief Der Geldbetrag der Ausgabe.
        public double Price { get; set; }

        /// @brief Die Kategorie der Ausgabe (z. B. Hardware, Software, Reise).
        public string Category { get; set; }

        /// @brief Der relative Pfad zur PDF-Rechnung (falls vorhanden).
        public string? Rechnung { get; set; }


    }
}
