using System.Diagnostics;
using Android.Gms.Common.ModuleInstall.Internal;
using Android.Hardware.Lights;
using Android.Media;
using Microsoft.Maui.Maps;
using VVVoyage.Models;
using static Android.Hardware.Camera;

namespace VVVoyage
{
    public partial class MainPage : ContentPage
    {
        List<Sight> sights;


        public MainPage()
        {
            InitializeComponent();
            InitialiseSights();

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);
          
            map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5)));

        }
    }
}
