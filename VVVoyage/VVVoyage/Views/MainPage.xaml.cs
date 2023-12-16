using System.Diagnostics;
using Microsoft.Maui.Maps;
using VVVoyage.Models;

namespace VVVoyage
{
    public partial class MainPage : ContentPage
    {
        List<Sight> sights;


        public MainPage()
        {
            InitializeComponent();
            InitialiseSights();
            CheckGPSAccess();

            foreach(var sight in sights)
            {
                map.Pins.Add(sight.SightPin);
            }

        }

        private void InitialiseSights()
        {
            Sight oldBuilding = new Sight("Oud VVV pand", new Location(51.592412225120796, 4.775692730162895), "Een heel mooi kerkje, of optimel drinkyoghurt passievrucht?");
            Sight loveSister = new Sight("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), "volkswagen polo 1.4 5000 rpm");
            Sight monument = new Sight("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), "dr oetker tonijn pizza");
            Sight lightHouse = new Sight("The Light House", new Location(51.584039168945026, 4.774673039583854), "oof");
            Sight castle = new Sight("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), "marktplaats koopjesjagen");
            Sight easterEgg = new Sight("Geheime Excelsior bijeenkomst", new Location(51.66222507319058, 4.739362181917831), "fujas");

            sights = new List<Sight> { oldBuilding, loveSister, monument, lightHouse, castle, easterEgg };
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
            try
            {
                base.OnAppearing();

                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.25)));

                OnStartListening();
            }
            catch (PermissionException ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
            
        async void OnStartListening()
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

        void LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(0.25)));
        }
    }
}
