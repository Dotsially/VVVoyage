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
    [QueryProperty(nameof(Tour), "Tour")]
    public partial class MainPage : ContentPage
    {
        private Tour _tour;
        public Tour Tour
        {
            get => _tour;
            set
            {
                _tour = value;
                OnPropertyChanged();
            }
        }

        private RootPageViewModel _viewModel;
        private CancellationTokenSource _cancellationTokenSource;

        public MainPage()
        {
            InitializeComponent();

            // Hides the App bar at the top of the screen
            Shell.SetNavBarIsVisible(this, false);
        }

        protected async override void OnAppearing()
        {
            _cancellationTokenSource = new();
            await Task.Delay(500);

            // Move map to Grote Kerk Breda
            map.MoveToRegion(new MapSpan(new Location(51.588833, 4.775278), 0.02, 0.02));

            _viewModel = new RootPageViewModel(
                new(Tour.Landmarks),
                map,
                new MapNavigator(Geolocation.Default, "AIzaSyBXG_XrA3JRTL58osjxd0DbqH563e2t84o"),
                new PopupNotifier(),
                new PushNotifier()
            );

            await _viewModel.CheckGPSAccess();

            await _viewModel.UpdateMapRepeatedly(_cancellationTokenSource.Token);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _cancellationTokenSource.Cancel();
        }

        private async void InstructionsButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("InstructionsPage");
        }

        private async void StopRouteButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
