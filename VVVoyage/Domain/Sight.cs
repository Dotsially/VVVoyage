using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    internal class Sight
    {
        
        public Pin SightPin { get; set; }
        public string SightDescription { get; set; }


        public Sight(string sightName, string sightDescription)
        {
            SightPin = new Pin
            {
                Label = "Bezienswaardigheid",
                Address = sightName,
                Type = PinType.Place,  
            };

            SightDescription = sightDescription;
        }

        public Sight(string sightLabel, string sightName, string sightDescription) 
        {
            SightPin = new Pin
            {
                Label = sightLabel,
                Address = sightName,
                Type = PinType.Place,
            };

            SightDescription = sightDescription;
        }

    }
}
