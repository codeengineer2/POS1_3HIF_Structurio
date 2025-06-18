using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// @file Timestamp_Json.cs
/// @brief JSON-Modellklasse für Zeitstempel-Kommunikation mit dem Backend.

namespace Structurio
{
    /// @class Timestamp_Json
    /// @brief Repräsentiert die serverseitige Struktur eines Zeitstempels im JSON-Format.
    public class Timestamp_Json
    {
        /// @brief Uhrzeit des Check-In (Format: "HH:mm:ss").
        public string checkin { get; set; }

        /// @brief Uhrzeit des Check-Out (Format: "HH:mm:ss").
        public string checkout { get; set; }

        /// @brief Datum des Check-In (Format: "yyyy-MM-dd").
        public string datum_in { get; set; }

        /// @brief Datum des Check-Out (Format: "yyyy-MM-dd").
        public string datum_out { get; set; }

        /// @brief Dauer der Arbeitszeit (Format: "hh:mm").
        public string duration { get; set; }

        /// @brief Zeitstempel-ID.
        public int zid { get; set; }

        /// @brief Benutzer-ID.
        public int uid { get; set; }

        /// @brief Projekt-ID.
        public int pid { get; set; }
    }
}
