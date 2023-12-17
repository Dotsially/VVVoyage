using System.Diagnostics;
using System.IO.IsolatedStorage;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using VVVoyage.Models;
using VVVoyage.Subsystems.Navigation;
using VVVoyage.Subsystems.Notification;

namespace VVVoyage
{
    public partial class MainPage : ContentPage
    {
        private readonly List<Sight> sights;

        private readonly INavigator _navigator;
        private readonly INotifier _popupNotifier = new PopupNotifier();

        public MainPage()
        {
            InitializeComponent();

            sights = InitialiseSights();

            _navigator = new MapNavigator(Geolocation.Default, "AIzaSyBXG_XrA3JRTL58osjxd0DbqH563e2t84o");

            // Move map to Grote Kerk Breda
            map.MoveToRegion(new MapSpan(new Location(51.588833, 4.775278), 0.02, 0.02));
        }

        private static List<Sight> InitialiseSights()
        {
            Sight oldBuilding = new("Oud VVV pand", new Location(51.592412225120796, 4.775692730162895), "Een heel mooi kerkje, of optimel drinkyoghurt passievrucht?");
            Sight loveSister = new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), "volkswagen polo 1.4 5000 rpm");
            Sight monument = new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), "dr oetker tonijn pizza");
            Sight lightHouse = new("The Light House", new Location(51.584039168945026, 4.774673039583854), "oof");
            Sight castle = new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), "marktplaats koopjesjagen");
            Sight easterEgg = new("Geheime Excelsior bijeenkomst", new Location(51.66222507319058, 4.739362181917831), "fujas");

            return [oldBuilding, loveSister, monument, lightHouse, castle, easterEgg];
        }

        async Task CheckGPSAccess()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>() | await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                // GPS location permission not granted let the user know.               
                await App.Current.MainPage.DisplayAlert("Permission Required", "GPS location permission is required to use this app.", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await UpdateMapRepeatedly();
        }

        private async Task UpdateMapRepeatedly()
        {
            List<Sight> visibleLandmarks = [];
            visibleLandmarks.Add(sights[0]);

            while (true)
            {
                Sight lastLandmark = visibleLandmarks.Last();
                MapUpdate? mapUpdate = await _navigator.UpdateMapAsync(lastLandmark);

                // If null, that means the map update has been canceled. So, this method should
                // not request any more map updates.
                // TODO add a route back to the main menu screen.
                if (mapUpdate == null) return;

                Debug.WriteLine("Map update called");

                map.Pins.Clear();
                map.MapElements.Clear();

                // Add visited landmarks to map
                DisplayLandmarks(visibleLandmarks);

                // Add route polyline to map
                DisplayRoutePolyline(mapUpdate.UserToLandmarkPolylineLocations);

                if (mapUpdate.IsUserCloseToLandmark)
                {
                    await _popupNotifier.ShowNotificationAsync(lastLandmark.SightDescription, lastLandmark.SightPin.Address, "Continue");

                    if (visibleLandmarks.Count < sights.Count)
                    {
                        Sight nextSight = sights[visibleLandmarks.Count];
                        visibleLandmarks.Add(nextSight);
                    }
                    else
                    {
                        // TODO add route back to the main menu screen.
                        await _popupNotifier.ShowNotificationAsync("Route is done.", "Finished", "OK");
                        return;
                    }
                }

                await Task.Delay(2000);
            }
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
                map.MapElements.Add(visitedLandmarkCircle);
            }

            // Add the circle and pin for the unvisited landmark,
            // which is the last element in the visibleLandmarks list.
            map.MapElements.Add(new Circle()
            {
                Center = visibleLandmarks.Last().SightPin.Location,
                Radius = new Distance(15),
                StrokeColor = Color.FromArgb("#88FF0000"),
                StrokeWidth = 8,
                FillColor = Color.FromArgb("#88FFC0CB")
            });
            map.Pins.Add(new Pin()
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

            map.MapElements.Add(polyline);
        }
    }
}
