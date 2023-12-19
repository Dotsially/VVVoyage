using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Models
{
    public class Tour(string name, string description, params Sight[] landmarks)
    {
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public ObservableCollection<Sight> Landmarks { get; set; } = new(landmarks);
    }
}
