using System.Diagnostics;
using System.IO.IsolatedStorage;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using VVVoyage.ViewModels;
using VVVoyage.Models;
using VVVoyage.Subsystems.Navigation;
using VVVoyage.Subsystems.Notification;
using Map = Microsoft.Maui.Controls.Maps.Map;
using VVVoyage.Resources.Localization;
using VVVoyage.Database;

namespace VVVoyage
{
    [QueryProperty(nameof(Tour), "Tour"), QueryProperty(nameof(LandmarkStartIndex), "LandmarkStartIndex")]
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

        private int _tourStartIndex;
        public int LandmarkStartIndex
        {
            get => _tourStartIndex;
            set
            {
                _tourStartIndex = value;
                OnPropertyChanged();
            }
        }

        private RootPageViewModel _viewModel;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly Map _map;
        private readonly IAppPreferences _appPreferences;
        private readonly INotifier _popupNotifier;

        public MainPage(IAppPreferences appPreferences, INotifier popupNotifier)
        {
            InitializeComponent();

            // Hides the App bar at the top of the screen
            Shell.SetNavBarIsVisible(this, false);

            _appPreferences = appPreferences;
            _popupNotifier = popupNotifier;
            _map = new() { IsShowingUser = true };

            Debug.WriteLine($"On MainPage, landmark start index is: {LandmarkStartIndex}");
        }

        protected async override void OnAppearing()
        {
            _cancellationTokenSource = new();

            _viewModel = new RootPageViewModel(
                new(Tour.Landmarks),
                LandmarkStartIndex,
                _map,
                _appPreferences,
                mapContainer,
                nextLandmarkView,
                buttonsView,
                new MapNavigator(Geolocation.Default, "AIzaSyBXG_XrA3JRTL58osjxd0DbqH563e2t84o"),
                _popupNotifier,
                new PushNotifier()
            );
            BindingContext = _viewModel;

            await HandlePermissions(_popupNotifier);

            await HandleLocationExceptions(_popupNotifier);

            mapContainer.Clear();
            mapContainer.Add(_map);

            // Move map to Grote Kerk Breda
            _map.MoveToRegion(new MapSpan(new Location(51.588833, 4.775278), 0.02, 0.02));

            instructionsBtn.IsEnabled = true;
            stopRouteBtn.IsEnabled = true;

            try
            {
                await _viewModel.UpdateMapRepeatedly(_cancellationTokenSource.Token);
            }
            catch (ApplicationException)
            {
                await _popupNotifier.ShowNotificationAsync(AppResources.Phone_Too_Far_Away_Message, AppResources.Phone_Too_Far_Away_Title, "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (FeatureNotEnabledException)
            {
                await _popupNotifier.ShowNotificationAsync(AppResources.Location_Disabled_Message, AppResources.Location_Disabled_Title, "OK");
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
            await _viewModel.OnInstructionsButtonClicked();
        }

        private async void StopRouteButton_Clicked(object sender, EventArgs e)
        {
            bool decision = await _popupNotifier.ShowNotificationAsync(AppResources.Stop_Route_Confirmation, AppResources.Confirmation, AppResources.Yes, AppResources.No);
            if (decision) await Shell.Current.GoToAsync("..");
        }

        private async Task HandlePermissions(INotifier notifier)
        {
            PermissionStatus status = PermissionStatus.Unknown;

            status = await _viewModel.CheckGPSAccess();


            // Permission not granted, let the user know.
            if (status != PermissionStatus.Granted)
            {
                await notifier.ShowNotificationAsync(AppResources.Location_Permission_Message, AppResources.Location_Permission_Title, "OK");

                await Shell.Current.GoToAsync("..");
            }
        }

        private async Task HandleLocationExceptions(INotifier notifier)
        {
            Location centerOfBreda = new(51.588833, 4.775278);

            try
            {
                if (!await _viewModel.IsUserInProximity(centerOfBreda, 5))
                {
                    await _popupNotifier.ShowNotificationAsync(AppResources.Phone_Too_Far_Away_Message, AppResources.Phone_Too_Far_Away_Title, "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (FeatureNotSupportedException)
            {
                await notifier.ShowNotificationAsync(AppResources.No_Location_Support_Message, AppResources.No_Location_Support_Title, "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (FeatureNotEnabledException)
            {
                // GPS not on   
                await _popupNotifier.ShowNotificationAsync(AppResources.Location_Disabled_Message, AppResources.Location_Disabled_Title, "OK");

                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
