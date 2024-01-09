using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Maps;
using ShapeControls = Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;
using VVVoyage.Models;
using VVVoyage.Resources.Localization;
using VVVoyage.Subsystems.Navigation;
using VVVoyage.Subsystems.Notification;
using Map = Microsoft.Maui.Controls.Maps.Map;
using VVVoyage.Database;

namespace VVVoyage.ViewModels
{
    public partial class RootPageViewModel : ObservableObject
    {
        private readonly Map _map;
        private readonly IAppPreferences _appPreferences;
        private readonly StackLayout _mapContainer;
        private readonly HorizontalStackLayout _nextLandmarkView;
        private readonly VerticalStackLayout _buttonsView;
        private readonly INavigator _navigator;
        private readonly INotifier _popupNotifier;
        private readonly INotifier _pushNotifier;

        private readonly List<Sight> _landmarks;
        private int _currentLandmarkIndex = 0;
        private bool _tourFinished = false;

        [ObservableProperty]
        private string landmarkName;

        [ObservableProperty]
        private string imageString;

        public RootPageViewModel(
            List<Sight> landmarks,
            int landmarkStartIndex,
            Map map,
            IAppPreferences appPreferences,
            StackLayout mapContainer, 
            HorizontalStackLayout nextLandmarkView,
            VerticalStackLayout buttonsView,
            INavigator navigator, 
            INotifier popupNotifier, 
            INotifier pushNotifier)
        {
            _landmarks = landmarks;
            _currentLandmarkIndex = landmarkStartIndex;
            _appPreferences = appPreferences;
            _map = map;
            _mapContainer = mapContainer;
            _nextLandmarkView = nextLandmarkView;
            _buttonsView = buttonsView;
            _navigator = navigator;
            _popupNotifier = popupNotifier;
            _pushNotifier = pushNotifier;

            Debug.WriteLine($"Starting tour at index {_currentLandmarkIndex} with landmark {_landmarks[_currentLandmarkIndex].SightPin.Address}");
        }

        public async Task<bool> IsUserInProximity(Location location, int maxDistanceKm)
        {
            try
            {
                Location userLocation = await _navigator.GetUserLocationAsync(new());
                return Distance.BetweenPositions(location, userLocation).Kilometers < maxDistanceKm;
            }
            catch (InvalidNavigationException) { return true; }
        }

        public async Task<PermissionStatus> CheckGPSAccess()
        {
            // Check the status before displaying the prompt.
            // If the permission was already granted, just return and don't request for it again.
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status == PermissionStatus.Granted) return PermissionStatus.Granted;

            // Request the permission.
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();

            return status;
        }

        public async Task UpdateMapRepeatedly(CancellationToken cancellationToken)
        {
            List<Sight> visibleLandmarks = [];
            visibleLandmarks.Add(_landmarks[_currentLandmarkIndex]);

            while (Shell.Current.CurrentPage is MainPage)
            {
                Sight lastLandmark = visibleLandmarks.Last();
                MapUpdate? mapUpdate = null;

                try
                {
                    mapUpdate = await _navigator.UpdateMapAsync(lastLandmark);
                }
                catch (InvalidNavigationException) { continue; }
                catch (WebException)
                {
                    bool quitTour = await _popupNotifier.ShowNotificationAsync(AppResources.No_Internet_Message, AppResources.No_Internet_Title, AppResources.No_Internet_Stop, AppResources.No_Internet_Continue);
                    if (quitTour)
                    {
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                    continue;
                }
                catch (HttpRequestException)
                {
                    bool quitTour = await _popupNotifier.ShowNotificationAsync(AppResources.No_Internet_Message, AppResources.No_Internet_Title, AppResources.No_Internet_Stop, AppResources.No_Internet_Continue);
                    if (quitTour)
                    {
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                    continue;
                }

                ImageString = lastLandmark.GetImageString();
                LandmarkName = lastLandmark.SightPin.Address;

                // If null, that means the map update has been canceled. So, this method should
                // not request any more map updates.
                if (mapUpdate == null) break;

                Debug.WriteLine("Map update called");

                _map.Pins.Clear();
                _map.MapElements.Clear();

                // Add visited landmarks to map
                DisplayLandmarks(visibleLandmarks);

                // Add route polyline to map
                DisplayRoutePolyline(mapUpdate.UserToLandmarkPolylineLocations);

                if (mapUpdate.IsUserCloseToLandmark)
                {
                    _nextLandmarkView.IsVisible = false;
                    _buttonsView.IsVisible = false;
                    _mapContainer.Clear();
                    _mapContainer.Add(GetLandmarkPopupView(lastLandmark.SightPin.Address, lastLandmark.SightDescription, lastLandmark.ImagePath));

                    await _pushNotifier.ShowNotificationAsync(lastLandmark.SightDescription, lastLandmark.SightPin.Address, "");

                    _currentLandmarkIndex++;

                    _appPreferences.SetPreference("lastLandmarkVisitedId", _currentLandmarkIndex);
                    _appPreferences.SetPreference("lastLandmarkVisitedDate", DateTime.Now.ToString("d/M/yyyy"));

                    if (_currentLandmarkIndex < _landmarks.Count)
                    {
                        Sight nextSight = _landmarks[_currentLandmarkIndex];
                        visibleLandmarks.Add(nextSight);
                    }
                    else
                    {
                        _tourFinished = true;

                        break;
                    }
                }
                else
                {
                    // Only wait this interval if the user did not reach a landmark.
                    // This way, when the user does reach a landmark, there is no/less
                    // delay when loading the next landmark
                    try { await Task.Delay(2000, cancellationToken); }
                    catch (TaskCanceledException) { break; }
                }
            }

            _navigator.CancelMapUpdate();

            Debug.WriteLine("Map updating stopped");
        }

        /// <summary>
        /// Displays a circle on the map on each visible landmark's location in their correct color.
        /// The last element in the visibleLandmarks list will be colored differently than the other
        /// landmarks.
        /// </summary>
        private void DisplayLandmarks(List<Sight> visibleLandmarks)
        {
            // Loop through all the visible landmarks, except the last one
            // (because the last landmark is unvisited and should have a different color).
            for (int i = 0; i < visibleLandmarks.Count - 1; i++)
            {
                Circle visitedLandmarkCircle = new()
                {
                    Center = visibleLandmarks[i].SightPin.Location,
                    Radius = new Distance(15),
                    StrokeColor = Color.FromArgb("#886CBF00"),
                    StrokeWidth = 8,
                    FillColor = Color.FromArgb("#88ABABAB")
                };
                _map.MapElements.Add(visitedLandmarkCircle);
            }

            // Add the circle and pin for the unvisited landmark,
            // which is the last element in the visibleLandmarks list.
            _map.MapElements.Add(new Circle()
            {
                Center = visibleLandmarks.Last().SightPin.Location,
                Radius = new Distance(15),
                StrokeColor = Color.FromArgb("#88FF0000"),
                StrokeWidth = 8,
                FillColor = Color.FromArgb("#88FFC0CB")
            });
            _map.Pins.Add(new Pin()
            {
                Label = "Landmark to reach",
                Location = visibleLandmarks.Last().SightPin.Location
            });
        }

        /// <summary>
        /// Displays the route polyline on the map between the user and the next landmark.
        /// </summary>
        private void DisplayRoutePolyline(List<Location> userToLandmarkPolylineLocations)
        {
            Polyline polyline = new()
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 5
            };

            // Add all locations in the polyline to the physical polyline
            foreach (var location in userToLandmarkPolylineLocations)
                polyline.Geopath.Add(location);

            _map.MapElements.Add(polyline);
        }

        public async Task OnInstructionsButtonClicked()
        {
            Dictionary<string, object> param = new() { { "LandmarkStartIndex", _currentLandmarkIndex } };

            await Shell.Current.GoToAsync("InstructionsPage", param);
        }

        private StackLayout GetLandmarkPopupView(string title, string description, string imageSource)
        {
            return new()
            {
                Children =
                {
                    new Label() { Text = title },
                    new Label() { Text = description },
                    new Border()
                    {
                        VerticalOptions = LayoutOptions.Start,
                        MaximumWidthRequest = 130,
                        MaximumHeightRequest = 130,
                        StrokeThickness = 0,
                        StrokeShape = new ShapeControls.RoundRectangle() { CornerRadius = 10 },
                        Content = new Image() { Aspect = Aspect.AspectFit, Source = imageSource }
                    },
                    new Button
                    {
                        Text = AppResources.Next_Landmark,
                        Command = new Command(async() =>
                        {
                            _mapContainer.Clear();
                            _mapContainer.Add(_map);
                            _nextLandmarkView.IsVisible = true;
                            _buttonsView.IsVisible = true;

                            if (_tourFinished)
                            {
                                await _pushNotifier.ShowNotificationAsync(AppResources.Tour_End_Message, AppResources.Tour_End_Title, "");

                                await _popupNotifier.ShowNotificationAsync(AppResources.Tour_End_Message, AppResources.Tour_End_Title, "OK");

                                await Shell.Current.GoToAsync("..");
                            }
                        })
                    }
                }
            };
        }
    }
}
