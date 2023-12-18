using System.Diagnostics;
using System.IO.IsolatedStorage;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using VVVoyage.ViewModels;
using VVVoyage.Models;
using VVVoyage.Subsystems.Navigation;
using VVVoyage.Subsystems.Notification;

namespace VVVoyage
{
    [QueryProperty(nameof(TourName), "tourName")]
    public partial class MainPage : ContentPage
    {
        private string _tourName;
        public string TourName
        {
            get => _tourName;
            set
            {
                _tourName = value;
                OnPropertyChanged();
            }
        }

        private RootPageViewModel _rootPageViewModel;

        public MainPage()
        {
            InitializeComponent();

            _rootPageViewModel = new RootPageViewModel(map);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.25)));

            _rootPageViewModel.OnStartListening();

            await _rootPageViewModel.UpdateMapRepeatedly();
        }
    }
}
