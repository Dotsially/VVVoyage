using Domain;
using System.Diagnostics;
using Microsoft.Maui.Maps;

namespace VVVoyage
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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
