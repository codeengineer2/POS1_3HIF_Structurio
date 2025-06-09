using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio
{
    public class Timestamp_Json
    {
        public string checkin { get; set; }
        public string checkout { get; set; }
        public string datum_in { get; set; }
        public string datum_out { get; set; }

        public string duration { get; set; }
        public int zid { get; set; }
        public int uid { get; set; }
        public int pid { get; set; }
    }
}
