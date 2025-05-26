using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio
{
    public class Timecheckin
    {
        public DateOnly Date { get; set; }
        public TimeOnly CheckIN { get; set; }
        public TimeOnly CheckOUT { get; set; }
        public string Duration { get; set; }
    }
}
