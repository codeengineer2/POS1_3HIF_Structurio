using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// @file Timecheckin.cs
/// @brief Modellklasse für einen einzelnen Arbeitszeitstempel (Check-in / Check-out).
namespace Structurio
{
    /// @class Timecheckin
    /// @brief Repräsentiert einen lokalen Zeitstempel mit Ein- und Ausstempel-Zeiten.
    public class Timecheckin
    {
        /// @brief Die eindeutige ID dieses Zeitstempels.
        public int Zid { get; set; }

        /// @brief Zeitstempel des Arbeitsbeginns (Check-In).
        public DateTime CheckIN { get; set; }

        /// @brief Zeitstempel des Arbeitsendes (Check-Out).
        public DateTime CheckOUT { get; set; }

        /// @brief Die berechnete Dauer zwischen CheckIN und CheckOUT im Format "hh:mm".
        public string Duration { get; set; }
    }
}
