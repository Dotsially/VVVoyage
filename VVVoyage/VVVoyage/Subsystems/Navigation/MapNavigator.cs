using Microsoft.Maui.Maps;
using PolylineEncoder.Net.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.Subsystems.Navigation
{
    // TODO make route class and replace with List<Sight>, and/or rename Sight to Landmark
    class MapNavigator(IGeolocation geolocationAPI, string googleMapsAPIKey) : INavigator
    {
        private readonly IGeolocation _geolocationAPI = geolocationAPI;
        private readonly string _googleMapsAPIKey = googleMapsAPIKey;

        // The cancellation token, used for cancelling a map update if needed
        private CancellationTokenSource? _cancellationTokenSource;

        // Whether the map is currently updating, to signal that it can be
        // cancelled while this variable is true.
        private bool _isUpdatingMap;

        /// <summary>
        /// Asynchronously gets the user's location, the next landmark (if necessary), whether the user is close enough to the landmark
        /// and the list of positions between the user's location and landmark picturing the route. These are put inside a MapUpdate object,
        /// which is then returned.
        /// </summary>
        /// <param name="landmarkToReach">The landmark which the user needs to reach next.</param>
        /// <returns>A MapUpdate object with all the necessary information. Or, when the map update is canceled, null.</returns>
        /// <exception cref="FeatureNotSupportedException">Whenever the phone does not support GPS location tracking.</exception>
        /// <exception cref="FeatureNotEnabledException">Whenever the phone has GPS but has not enabled it.</exception>
        /// <exception cref="PermissionException">Whenever this app does not have the user's permission to track their location.</exception>
        /// <exception cref="WebException">Whenever the user does not have internet access (the hostname cannot be resolved).</exception>
        /// <exception cref="HttpRequestException">When something goes wrong with the HTTP request to the Google Directions API.</exception>
        /// <exception cref="InvalidNavigationException">Any other error that occurs when attempting to retrieve the user's location.</exception>
        public async Task<MapUpdate?> UpdateMapAsync(Sight landmarkToReach)
        {
            _isUpdatingMap = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                Location userLocation = await GetUserLocationAsync(_cancellationTokenSource.Token);

                bool isUserCloseToLandmark = IsUserCloseEnough(userLocation, landmarkToReach);

                List<Location> userToLandmarkPolylineLocations = await GetRoutePolyline(userLocation, landmarkToReach, _cancellationTokenSource.Token);

                // If the cancellation token was cancelled, return null, otherwise return a MapUpdate.
                if (_cancellationTokenSource.IsCancellationRequested) return null;
                else return new MapUpdate(userLocation, isUserCloseToLandmark, userToLandmarkPolylineLocations);
            }
            catch (Exception)
            {
                // Re-throw the same exception, so the finally block can be executed.
                throw;
            }
            finally
            {
                // Set this to false for cleanup
                _isUpdatingMap = false;
            }
        }

        /// <summary>
        /// Asynchronously gets the user's location using the GPS module.
        /// </summary>
        /// <returns>The user's location.</returns>
        /// <exception cref="FeatureNotSupportedException">Whenever the phone does not support GPS location tracking.</exception>
        /// <exception cref="FeatureNotEnabledException">Whenever the phone has GPS but has not enabled it.</exception>
        /// <exception cref="PermissionException">Whenever this app does not have the user's permission to track their location.</exception>
        /// <exception cref="InvalidNavigationException">Any other error that occurs when attempting to reetrieve the user's location.</exception>
        public async Task<Location> GetUserLocationAsync(CancellationToken cancellationToken)
        {
            GeolocationRequest request = new(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

            Location? userLocation = await _geolocationAPI.GetLocationAsync(request, cancellationToken);

            if (userLocation != null) return userLocation;
            else throw new InvalidNavigationException("Could not get location from this phone."); // Something unknown went wrong
        }

        /// <summary>
        /// Calculates the distance between the user and the landmark to reach. Determines if the user is close enough to the landmark.
        /// </summary>
        /// <returns>Whether the user is X meters away from the landmark.</returns>
        private bool IsUserCloseEnough(Location userLocation, Sight landmark)
        {
            Location landmarkLocation = landmark.SightPin.Location;

            // TODO think about replacing this calculation with one that accounts for roads.
            double distanceKM = Location.CalculateDistance(userLocation, landmarkLocation, DistanceUnits.Kilometers);

            return distanceKM <= 0.015;
        }

        /// <summary>
        /// Gets the encoded polyline from the Google Directions API between the user's location and the current landmark's location.
        /// </summary>
        /// <exception cref="WebException">Whenever the user does not have internet access (the hostname cannot be resolved).</exception>
        /// <exception cref="HttpRequestException">When something goes wrong with the HTTP request to the Google Directions API.</exception>
        private async Task<List<Location>> GetRoutePolyline(Location userLocation, Sight landmarkToReach, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            List<Location> locations = [];
            Location landmarkLocation = landmarkToReach.SightPin.Location;

            // In the Netherlands, converting a double value to string will result in the dot
            // being replaced by a comma (so 4.5 becomes 4,5). The Google Directions API does
            // not accept this, so we need to convert them ourselves.
            // Also, the comma between latitude and longitude needs to be replaced by %2C for
            // the URL to work.
            string userLocationURLString = $"{userLocation.Latitude.ToString().Replace(',', '.')}%2C{userLocation.Longitude.ToString().Replace(',', '.')}";
            string landmarkLocationURLString = $"{landmarkLocation.Latitude.ToString().Replace(',', '.')}%2C{landmarkLocation.Longitude.ToString().Replace(',', '.')}";

            // Request URL contains the user's location and the landmark's location, and requests for the route between them.
            var requestURL = $"https://maps.googleapis.com/maps/api/directions/json?origin={userLocationURLString}&destination={landmarkLocationURLString}&mode=walking&key={_googleMapsAPIKey}";
            
            // Make the request and receive the response
            var response = await client.GetAsync(requestURL, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                // TODO add more robust error handling
                return [];
            }

            // Read response as JSON
            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken);

            if (jsonResponse!["status"]!.ToString() == "ZERO_RESULTS")
                throw new ApplicationException("No route possible");

            // Get the encoded polyline value from the response
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

        /// <summary>
        /// Cancels the map update if it is in progress.
        /// </summary>
        public void CancelMapUpdate()
        {
            if (_isUpdatingMap && _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();
        }
    }
}
