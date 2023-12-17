using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using System.Windows.Input;
using VVVoyage.Models;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace VVVoyage.ViewModels
{
    public class RootPageViewModel
    {
        private List<Sight> sights;
        private Map map;

        public RootPageViewModel(Map map)
        {
            CheckGPSAccess();
            this.map = map;
            InitialiseSights();
            DrawSights();
        }

        private async void InitialiseSights()
        {
            Sight oldBuilding = new Sight("Oud VVV pand", new Location(51.592412225120796, 4.775692730162895), "Een heel mooi kerkje, of optimel drinkyoghurt passievrucht?");
            Sight loveSister = new Sight("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), "volkswagen polo 1.4 5000 rpm");
            Sight monument = new Sight("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), "dr oetker tonijn pizza");
            Sight lightHouse = new Sight("The Light House", new Location(51.584039168945026, 4.774673039583854), "oof");
            Sight castle = new Sight("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), "marktplaats koopjesjagen");
            Sight easterEgg = new Sight("Geheime Excelsior bijeenkomst", new Location(51.66222507319058, 4.739362181917831), "fujas");

            sights = new List<Sight> { oldBuilding, loveSister, monument, lightHouse, castle, easterEgg };
        }

        private void DrawSights()
        {
            foreach (var sight in sights)
            {
                map.Pins.Add(sight.SightPin);

                Circle circle = new Circle
                {
                    Center = sight.SightPin.Location,
                    Radius = new Distance(15),
                    StrokeColor = Color.FromArgb("#88FF0000"),
                    StrokeWidth = 8,
                    FillColor = Color.FromArgb("#88FFC0CB")
                };

                map.MapElements.Add(circle);
            }
        }

        private void DrawSightsWithout(Sight sightToChange)
        {
            foreach (var sight in sights)
            {
                if (!sight.Equals(sightToChange))
                {
                    map.Pins.Add(sight.SightPin);

                    Circle circle = new Circle
                    {
                        Center = sight.SightPin.Location,
                        Radius = new Distance(15),
                        StrokeColor = Color.FromArgb("#88FF0000"),
                        StrokeWidth = 8,
                        FillColor = Color.FromArgb("#88FFC0CB")
                    };

                    map.MapElements.Add(circle);
                }
                else
                {
                    map.Pins.Add(sight.SightPin);

                    Circle circle = new Circle
                    {
                        Center = sight.SightPin.Location,
                        Radius = new Distance(15),
                        StrokeColor = Color.FromArgb("#16288c"),
                        StrokeWidth = 8,
                        FillColor = Color.FromArgb("#2949ff")
                    };

                    map.MapElements.Add(circle);
                }
            }
        }

        private void SightColorChangerIfClose(Location newUserLocation)
        {
            foreach (var sight in sights)
            {
                double distance = CalculateDistance(newUserLocation.Latitude, newUserLocation.Longitude, sight.SightPin.Location.Latitude, sight.SightPin.Location.Longitude);

                // if the distance is smaller than 15 meters then the user is close to the sight.
                if (distance < 0.17)
                {
                    // TODO: <Wessel> using Plugin.LocalNotification;

                    map.MapElements.Clear();
                    DrawSightsWithout(sight);
                }
            }
        }

        public async Task OnStartListening()
        {
            try
            {
                Geolocation.LocationChanged += LocationChanged;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.High);
                await Geolocation.StartListeningForegroundAsync(request);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private async void LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(0.25)));

            SightColorChangerIfClose(e.Location);
        }

        private double CalculateDistance(double userLat, double userLong, double stationLat, double stationLong)
        {
            double distance = Location.CalculateDistance(userLat, userLong, stationLat, stationLong, DistanceUnits.Kilometers);
            return distance;
        }

        private async Task CheckGPSAccess()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>() | await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                // GPS location permission not granted let the user know.               
                await App.Current.MainPage.DisplayAlert("Permission Required", "GPS location permission is required to use this app.", "OK");
            }

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);
            }
            catch(FeatureNotEnabledException ex)
            {
                // GPS not on   
                await App.Current.MainPage.DisplayAlert("GPS Required", "GPS is required but turned off now :c", "OK");
            }
            
        }
    }
}
