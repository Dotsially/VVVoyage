using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Models
{
    public class Tour(string name, string description)
    {
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
    }
}
