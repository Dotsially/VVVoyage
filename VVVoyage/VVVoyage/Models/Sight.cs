using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Resources.Localization;

namespace VVVoyage.Models
{
    public class Sight
    {
        
        public Pin SightPin { get; set; }
        public string SightDescription { get; set; }
        public string ImagePath { get; set; }

        private string imageString;
        public int Id { get; }

        public Sight(int id, string sightName, Location location, string sightDescription, string imageString)
        {
            SightPin = new Pin
            {
                Label = AppResources.Landmark_Default_Label,
                Address = sightName,
                Type = PinType.Place,  
                Location = location
            };

            SightDescription = sightDescription;
            Id = id;

            this.imageString = imageString;

            ImagePath = imageString;
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

        public string GetImageString()
        {
            return imageString;
        }
    }
}
