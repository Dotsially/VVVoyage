using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Models
{
    public class Tour
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Sight> Landmarks { get; set; }
        public bool IsRandomTour { get; set; }

        public Tour(string name, string description, bool isRandomTour, params Sight[] landmarks)
        {
            Name = name;
            Description = description;
            Landmarks = new(landmarks);
            IsRandomTour = isRandomTour;
        }

        public Tour(string name, string description, params Sight[] landmarks)
        {
            Name = name;
            Description = description;
            Landmarks = new(landmarks);
            IsRandomTour = false;
        }
    }
}
