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
            INotifier popupNotifier = new PopupNotifier();

            await Task.Delay(500);

            // Move map to Grote Kerk Breda
            map.MoveToRegion(new MapSpan(new Location(51.588833, 4.775278), 0.02, 0.02));

            _viewModel = new RootPageViewModel(
                new(Tour.Landmarks),
                map,
                new MapNavigator(Geolocation.Default, "AIzaSyBXG_XrA3JRTL58osjxd0DbqH563e2t84o"),
                popupNotifier,
                new PushNotifier()
            );

            await HandlePermissions(popupNotifier);

            await HandleUserTooFarAway(popupNotifier);

            try
            {
                await _viewModel.UpdateMapRepeatedly(_cancellationTokenSource.Token);
            }
            catch (ApplicationException)
            {
                await popupNotifier.ShowNotificationAsync("The tour cannot be started, because the phone is too far away from Breda! Hint: if you're running an emulator, set the emulator location to somewhere in Breda in the emulator settings.", "Too far away", "OK");
                await Shell.Current.GoToAsync("..");
            }
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

        private async Task HandlePermissions(INotifier notifier)
        {
            PermissionStatus status = PermissionStatus.Unknown;

            try
            {
                status = await _viewModel.CheckGPSAccess();
            }
            catch (FeatureNotSupportedException)
            {
                await notifier.ShowNotificationAsync("GPS not supported", "The app cannot work on this phone, since it does not have a GPS.", "OK");

                await Shell.Current.GoToAsync("MainMenuPage");
            }
            catch (FeatureNotEnabledException)
            {
                // GPS not on   
                await notifier.ShowNotificationAsync("GPS Required", "GPS is required but turned off now :c", "OK");

                await Shell.Current.GoToAsync("MainMenuPage");
            }

            // Permission not granted, let the user know.
            if (status != PermissionStatus.Granted)
            {
                await notifier.ShowNotificationAsync("Permission Required", "GPS location permission is required to use this app.", "OK");

                await Shell.Current.GoToAsync("MainMenuPage");
            }
        }

        private async Task HandleUserTooFarAway(INotifier notifier)
        {
            Location centerOfBreda = new(51.588833, 4.775278);

            if (!await _viewModel.IsUserInProximity(centerOfBreda, 5))
            {
                await notifier.ShowNotificationAsync("The tour cannot be started, because the phone is too far away from Breda! Hint: if you're running an emulator, set the emulator location to somewhere in Breda in the emulator settings.", "Too far away", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
