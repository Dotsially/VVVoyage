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
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            await Task.Delay(500);

            // Move map to Grote Kerk Breda
            map.MoveToRegion(new MapSpan(new Location(51.588833, 4.775278), 0.02, 0.02));

            _viewModel = new RootPageViewModel(
                new(Tour.Landmarks),
                map,
                new MapNavigator(Geolocation.Default, "AIzaSyBXG_XrA3JRTL58osjxd0DbqH563e2t84o"),
                new PopupNotifier()
            );

            await _viewModel.CheckGPSAccess();

            await _viewModel.UpdateMapRepeatedly(_cancellationTokenSource.Token);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _cancellationTokenSource.Cancel();
        }
    }
}
