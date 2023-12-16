using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.Subsystems.Navigation
{
    class MapUpdate(
        Location userLocation,
        Sight landmarkToReach,
        bool isUserCloseToLandmark,
        List<Location> userToLandmarkPolylineLocations
    )
    {
        public readonly Location UserLocation = userLocation;
        public readonly Sight LandmarkToReach = landmarkToReach;
        public readonly bool IsUserCloseToLandmark = isUserCloseToLandmark;
        public readonly List<Location> UserToLandmarkPolylineLocations = userToLandmarkPolylineLocations;
    }
}
