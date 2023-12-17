using System.Diagnostics;
using Microsoft.Maui.Maps;
using VVVoyage.ViewModels;

namespace VVVoyage
{
    public partial class MainPage : ContentPage
    {
        private RootPageViewModel _rootPageViewModel;

        public MainPage()
        {
            InitializeComponent();
            _rootPageViewModel = new RootPageViewModel(map);
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.25)));

                _rootPageViewModel.OnStartListening();
            }
            catch (PermissionException ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
