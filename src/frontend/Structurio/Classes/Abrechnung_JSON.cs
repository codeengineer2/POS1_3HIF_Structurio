using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio
{
    public class Abrechnung_JSON
    {

            public int Aid { get; set; }
            public int Uid { get; set; }
            public int Pid { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public double Price { get; set; }
            public string Category { get; set; }
            public string? Rechnung { get; set; }
         

    }
}
