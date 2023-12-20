using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using System.Windows.Input;
using VVVoyage.Models;
using VVVoyage.Subsystems.Navigation;
using VVVoyage.Subsystems.Notification;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace VVVoyage.ViewModels
{
    public class RootPageViewModel(List<Sight> landmarks, Map map, INavigator navigator, INotifier popupNotifier, INotifier pushNotifier)
    {
        private readonly Map _map = map;
        private readonly INavigator _navigator = navigator;
        private readonly INotifier _popupNotifier = popupNotifier;
        private readonly INotifier _pushNotifier = pushNotifier;

        private readonly List<Sight> _landmarks = landmarks;

        public async Task<bool> IsUserInProximity(Location location, int maxDistanceKm)
        {
            Location userLocation = await _navigator.GetUserLocationAsync(new());

            return Distance.BetweenPositions(location, userLocation).Kilometers < maxDistanceKm;
        }

        public async Task<PermissionStatus> CheckGPSAccess()
        {
            // Check the status before displaying the prompt.
            // If the permission was already granted, just return and don't request for it again.
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status == PermissionStatus.Granted) return PermissionStatus.Granted;

            // Request the permission.
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();

            return status;
        }

        public async Task UpdateMapRepeatedly(CancellationToken cancellationToken)
        {
            List<Sight> visibleLandmarks = [];
            visibleLandmarks.Add(_landmarks[0]);

            while (Shell.Current.CurrentPage is MainPage)
            {
                Sight lastLandmark = visibleLandmarks.Last();
                MapUpdate? mapUpdate = await _navigator.UpdateMapAsync(lastLandmark);

                // If null, that means the map update has been canceled. So, this method should
                // not request any more map updates.
                if (mapUpdate == null) break;

                Debug.WriteLine("Map update called");

                _map.Pins.Clear();
                _map.MapElements.Clear();

                // Add visited landmarks to map
                DisplayLandmarks(visibleLandmarks);

                // Add route polyline to map
                DisplayRoutePolyline(mapUpdate.UserToLandmarkPolylineLocations);

                if (mapUpdate.IsUserCloseToLandmark)
                {
                    await _pushNotifier.ShowNotificationAsync(lastLandmark.SightDescription, lastLandmark.SightPin.Address, "");

                    await _popupNotifier.ShowNotificationAsync(lastLandmark.SightDescription, lastLandmark.SightPin.Address, "Continue");

                    if (visibleLandmarks.Count < _landmarks.Count)
                    {
                        Sight nextSight = _landmarks[visibleLandmarks.Count];
                        visibleLandmarks.Add(nextSight);
                    }
                    else
                    {
                        await _pushNotifier.ShowNotificationAsync("You have finished the tour! We hope you had a wonderful experience!", "Tour end", "");
                        
                        await _popupNotifier.ShowNotificationAsync("Route is done.", "Finished", "OK");

                        await Shell.Current.GoToAsync("..");

                        break;
                    }
                }
                else
                {
                    // Only wait this interval if the user did not reach a landmark.
                    // This way, when the user does reach a landmark, there is no/less
                    // delay when loading the next landmark
                    try { await Task.Delay(2000, cancellationToken); }
                    catch (TaskCanceledException) { break; }
                }
            }

            _navigator.CancelMapUpdate();

            Debug.WriteLine("Map updating stopped");
        }

        /// <summary>
        /// Displays a circle on the map on each visible landmark's location in their correct color.
        /// The last element in the visibleLandmarks list will be colored differently than the other
        /// landmarks.
        /// </summary>
        private void DisplayLandmarks(List<Sight> visibleLandmarks)
        {
            // Loop through all the visible landmarks, except the last one
            // (because the last landmark is unvisited and should have a different color).
            for (int i = 0; i < visibleLandmarks.Count - 1; i++)
            {
                Circle visitedLandmarkCircle = new()
                {
                    Center = visibleLandmarks[i].SightPin.Location,
                    Radius = new Distance(15),
                    StrokeColor = Color.FromArgb("#886CBF00"),
                    StrokeWidth = 8,
                    FillColor = Color.FromArgb("#88ABABAB")
                };
                _map.MapElements.Add(visitedLandmarkCircle);
            }

            // Add the circle and pin for the unvisited landmark,
            // which is the last element in the visibleLandmarks list.
            _map.MapElements.Add(new Circle()
            {
                Center = visibleLandmarks.Last().SightPin.Location,
                Radius = new Distance(15),
                StrokeColor = Color.FromArgb("#88FF0000"),
                StrokeWidth = 8,
                FillColor = Color.FromArgb("#88FFC0CB")
            });
            _map.Pins.Add(new Pin()
            {
                Label = "Landmark to reach",
                Location = visibleLandmarks.Last().SightPin.Location
            });
        }

        /// <summary>
        /// Displays the route polyline on the map between the user and the next landmark.
        /// </summary>
        private void DisplayRoutePolyline(List<Location> userToLandmarkPolylineLocations)
        {
            Polyline polyline = new()
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 5
            };

            // Add all locations in the polyline to the physical polyline
            foreach (var location in userToLandmarkPolylineLocations)
                polyline.Geopath.Add(location);

            _map.MapElements.Add(polyline);
        }
    }
}
