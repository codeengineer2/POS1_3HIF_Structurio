using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public User User { get; set; }
        public List<Project> Projects { get; set; }
    }
}