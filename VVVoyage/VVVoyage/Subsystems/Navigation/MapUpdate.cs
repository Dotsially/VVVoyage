using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.Subsystems.Navigation
{
    public class MapUpdate(
        Location userLocation,
        bool isUserCloseToLandmark,
        List<Location> userToLandmarkPolylineLocations
    )
    {
        public readonly Location UserLocation = userLocation;
        public readonly bool IsUserCloseToLandmark = isUserCloseToLandmark;
        public readonly List<Location> UserToLandmarkPolylineLocations = userToLandmarkPolylineLocations;
    }
}
