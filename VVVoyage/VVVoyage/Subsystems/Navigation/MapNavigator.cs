﻿using Microsoft.Maui.Maps;
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
    // TODO make route class and replace with List<Sight>, and/or rename Sight to Landmark
    class MapNavigator : INavigator
    {
        // The accuracy in which the GPS displays the location. Fiddle around with this to see
        // what works best.
        private readonly GeolocationAccuracy LOCATION_ACCURACY = GeolocationAccuracy.Medium;

        // This many seconds after requesting the user's location, the GPS location request
        // will be cancelled. To prevent long waiting scenarios.
        private readonly int LOCATION_REQUEST_TIMEOUT_SECONDS = 10;

        // Threshold at which a new landmark is triggered. According to
        // documentation, this should be at fifteen meters.
        private readonly int SIGHT_TRIGGER_DISTANCE_METERS = 15;

        private readonly IGeolocation _geolocationAPI;
        private readonly List<Sight> _route;
        private readonly string _googleMapsAPIKey;
        private Sight _landmarkToReach;

        // The cancellation token, used for cancelling a map update if needed
        private CancellationTokenSource? _cancellationTokenSource;

        // Whether the map is currently updating, to signal that it can be
        // cancelled while this variable is true.
        private bool _isUpdatingMap;

        /// <exception cref="ArgumentOutOfRangeException">Whenever the route list contains zero landmarks</exception>
        public MapNavigator(IGeolocation geolocationAPI, List<Sight> route, string googleMapsAPIKey)
        {
            _geolocationAPI = geolocationAPI;
            _route = route;
            _googleMapsAPIKey = googleMapsAPIKey;

            if (route.Count == 0) throw new ArgumentOutOfRangeException(nameof(route), "The route cannot have zero landmarks defined!");
            else _landmarkToReach = route[0];
        }

        /// <summary>
        /// Asynchronously gets the user's location, the next landmark (if necessary), whether the user is close enough to the landmark
        /// and the list of positions between the user's location and landmark picturing the route. These are put inside a MapUpdate object,
        /// which is then returned.
        /// </summary>
        /// <returns>A MapUpdate object with all the necessary information. Or, when the map update is canceled, null.</returns>
        /// <exception cref="FeatureNotSupportedException">Whenever the phone does not support GPS location tracking.</exception>
        /// <exception cref="FeatureNotEnabledException">Whenever the phone has GPS but has not enabled it.</exception>
        /// <exception cref="PermissionException">Whenever this app does not have the user's permission to track their location.</exception>
        /// <exception cref="Exception">Any other error that occurs when attempting to retrieve the user's location.</exception>
        public async Task<MapUpdate?> UpdateMapAsync()
        {
            _isUpdatingMap = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                Location userLocation = await GetUserLocation(_cancellationTokenSource.Token);

                bool isUserCloseToLandmark = IsUserCloseEnough(userLocation);

                List<Location> userToLandmarkPolylineLocations = await GetRoutePolyline(userLocation, _cancellationTokenSource.Token);

                // If the cancellation token was cancelled, return null, otherwise return a MapUpdate.
                if (_cancellationTokenSource.IsCancellationRequested) return null;
                else return new MapUpdate(userLocation, _landmarkToReach, isUserCloseToLandmark, userToLandmarkPolylineLocations);
            }
            catch (Exception)
            {
                // Re-throw the exception, so the finally block can be executed.
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
        /// <exception cref="HttpRequestException">When something goes wrong with the HTTP request to the Google Directions API.</exception>
        /// <exception cref="Exception">Any other error that occurs when attempting to reetrieve the user's location.</exception>
        private async Task<Location> GetUserLocation(CancellationToken cancellationToken)
        {
            GeolocationRequest request = new(LOCATION_ACCURACY, TimeSpan.FromSeconds(LOCATION_REQUEST_TIMEOUT_SECONDS));

            Location? userLocation = await _geolocationAPI.GetLocationAsync(request, cancellationToken);

            if (userLocation != null) return userLocation;
            else throw new Exception("Could not get location from this phone."); // Something unknown went wrong
        }

        /// <summary>
        /// Calculates the distance between the user and the landmark to reach. Determines if the user is close enough to the landmark.
        /// </summary>
        /// <returns>Whether the user is X meters away from the landmark.</returns>
        private bool IsUserCloseEnough(Location userLocation)
        {
            Location landmarkLocation = _landmarkToReach.SightPin.Location;

            // TODO think about replacing this calculation with one that accounts for roads.
            double distanceKM = Location.CalculateDistance(userLocation, landmarkLocation, DistanceUnits.Kilometers);

            if (distanceKM <= SIGHT_TRIGGER_DISTANCE_METERS)
            {
                // Update the landmark to reach by getting the next landmark in the list.
                int nextIndex = _route.IndexOf(_landmarkToReach) + 1;
                _landmarkToReach = _route[nextIndex];

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the encoded polyline from the Google Directions API between the user's location and the current landmark's location.
        /// </summary>
        /// <exception cref="HttpRequestException">When something goes wrong with the HTTP request to the Google Directions API.</exception>
        private async Task<List<Location>> GetRoutePolyline(Location userLocation, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();

            List<Location> locations = [];
            Location landmarkLocation = _landmarkToReach.SightPin.Location;

            // Request URL contains the user's location and the landmark's location, and requests for the route between them.
            var requestURL = $"https://maps.googleapis.com/maps/api/directions/json?origin={userLocation.Latitude}%2C{userLocation.Longitude}&destination={landmarkLocation.Latitude}%2C{landmarkLocation.Longitude}&mode=walking&key={_googleMapsAPIKey}";
            
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
