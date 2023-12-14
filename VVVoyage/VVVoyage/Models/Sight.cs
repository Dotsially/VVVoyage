using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Models
{
    internal class Sight
    {
        
        public Pin SightPin { get; set; }
        public string SightDescription { get; set; }


        public Sight(string sightName, Location location, string sightDescription)
        {
            SightPin = new Pin
            {
                Label = "Bezienswaardigheid",
                Address = sightName,
                Type = PinType.Place,  
                Location = location
            };

            SightDescription = sightDescription;
        }

        public Sight(string sightLabel, string sightName, Location location, string sightDescription) 
        {
            SightPin = new Pin
            {
                Label = sightLabel,
                Address = sightName,
                Type = PinType.Place,
                Location = location
            };

            SightDescription = sightDescription;
        }

    }
}
