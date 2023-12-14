using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
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
          
            map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.25)));

            OnStartListening();
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
