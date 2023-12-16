using Microsoft.Maui.Maps;
using PolylineEncoder.Net.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.Subsystems.Navigation
{
    // TODO make route class and replace with List<Sight>
    class MapNavigator : INavigator
    {
        private readonly GeolocationAccuracy LOCATION_ACCURACY = GeolocationAccuracy.Medium;
        private readonly int LOCATION_REQUEST_TIMEOUT_SECONDS = 10;
        private readonly int SIGHT_TRIGGER_DISTANCE_METERS = 15;

        private readonly IGeolocation _geolocationAPI;
        private readonly List<Sight> _route;
        private readonly string _googleMapsAPIKey;
        private Sight _landmarkToReach;
        private CancellationTokenSource? _cancellation;
        private bool _isCheckingLocation;

        /// <exception cref="ArgumentOutOfRangeException">Whenever the route list contains zero landmarks</exception>
        public MapNavigator(IGeolocation geolocationAPI, List<Sight> route, string googleMapsAPIKey)
        {
            _geolocationAPI = geolocationAPI;
            _route = route;
            _googleMapsAPIKey = googleMapsAPIKey;

            if (route.Count == 0) throw new ArgumentOutOfRangeException(nameof(route), "The route cannot have zero landmarks defined!");
            else _landmarkToReach = route[0];
        }

        /// <exception cref="FeatureNotSupportedException">Whenever the phone does not support GPS location tracking.</exception>
        /// <exception cref="FeatureNotEnabledException">Whenever the phone has GPS but has not enabled it.</exception>
        /// <exception cref="PermissionException">Whenever this app does not have the user's permission to track their location.</exception>
        /// <exception cref="Exception">Any other error that occurs when attempting to retrieve the user's location.</exception>
        public async Task<MapUpdate> UpdateMapAsync()
        {
            Location userLocation = await GetUserLocation();

            bool isUserCloseToLandmark = IsUserCloseEnough(userLocation);

            List<Location> userToLandmarkPolylineLocations = await GetRoutePolyline(userLocation);

            return new(userLocation, _landmarkToReach, isUserCloseToLandmark, userToLandmarkPolylineLocations);
        }

        /// <summary>
        /// Asynchronously gets the user's location using the GPS module.
        /// </summary>
        /// <returns>The user's location.</returns>
        /// <exception cref="FeatureNotSupportedException">Whenever the phone does not support GPS location tracking.</exception>
        /// <exception cref="FeatureNotEnabledException">Whenever the phone has GPS but has not enabled it.</exception>
        /// <exception cref="PermissionException">Whenever this app does not have the user's permission to track their location.</exception>
        /// <exception cref="Exception">Any other error that occurs when attempting to reetrieve the user's location.</exception>
        private async Task<Location> GetUserLocation()
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new(LOCATION_ACCURACY, TimeSpan.FromSeconds(LOCATION_REQUEST_TIMEOUT_SECONDS));
            _cancellation = new();

            Location? userLocation = await _geolocationAPI.GetLocationAsync(request, _cancellation.Token);

            _isCheckingLocation = false;

            if (userLocation != null) return userLocation;
            else throw new Exception("Could not get location from this phone.");
        }

        public void CancelMapUpdate()
        {
            if (_isCheckingLocation && _cancellation != null && _cancellation.IsCancellationRequested)
                _cancellation.Cancel();
        }

        private bool IsUserCloseEnough(Location userLocation)
        {
            Location landmarkLocation = _landmarkToReach.SightPin.Location;

            double distanceKM = Location.CalculateDistance(userLocation, landmarkLocation, DistanceUnits.Kilometers);

            if (distanceKM <= SIGHT_TRIGGER_DISTANCE_METERS)
            {
                int nextIndex = _route.IndexOf(_landmarkToReach) + 1;
                _landmarkToReach = _route[nextIndex];

                return true;
            }

            return false;
        }

        private async Task<List<Location>> GetRoutePolyline(Location userLocation)
        {
            List<Location> locations = [];

            using var client = new HttpClient();

            Location landmarkLocation = _landmarkToReach.SightPin.Location;
            var requestURL = $"https://maps.googleapis.com/maps/api/directions/json?origin={userLocation.Latitude}%2C{userLocation.Longitude}&destination={landmarkLocation.Latitude}%2C{landmarkLocation.Longitude}&mode=walking&key={_googleMapsAPIKey}";
            // Receive response
            var response = await client.GetAsync(requestURL);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                // TODO add more robust error handling
                return locations;
            }

            // Read response as JSON
            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>();

            // TODO more robust error handling
            var encodedPolyline =
                // Get the routes array
                jsonResponse!["routes"]!.AsArray()
                // Get the first JsonObject from the array
                [0]!.AsObject()
                // Get the overview_polyline object
                ["overview_polyline"]!.AsObject()
                // Get the points value inside the object as a string
                ["points"]!.ToString();

            // Decode polyline using the NuGet package
            PolylineUtility decoder = new();
            var coordinates = decoder.Decode(encodedPolyline);

            // Add all positions of the route to a list
            foreach (var coordinate in coordinates)
            {
                locations.Add(new Location(coordinate.Latitude, coordinate.Longitude));
            }
            return locations;
        }
    }
}
