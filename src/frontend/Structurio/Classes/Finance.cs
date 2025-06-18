using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// @file Finance.cs
/// @brief Modellklasse zur Darstellung einer Abrechnungskostenposition.
namespace Structurio
{
    /// @class Finance
    /// @brief Repräsentiert eine einzelne Kostenposition innerhalb eines Projekts.
    public class Finance
    {

        /// @brief Der Name oder die Beschreibung der Ausgabe.
        public string Name { get; set; }

        /// @brief Der Preis oder Betrag der Ausgabe.
        public double Preis { get; set; }

        /// @brief Das Datum der Ausgabe.
        public DateTime Datum { get; set; }

        /// @brief Die zugewiesene Kategorie der Ausgabe (z. B. "Hardware", "Lizenzen").
        public string Kategorie { get; set; }

        /// @brief Der relative Pfad zur zugehörigen PDF-Rechnung.
        public string Rechnung { get; set; }



    }
}
