using System.Diagnostics;
using VVVoyage.Database;
using VVVoyage.Models;
using VVVoyage.Resources.Localization;
using VVVoyage.Subsystems.Notification;
using VVVoyage.ViewModels;

namespace VVVoyage.Views;

public partial class MainMenuPage : ContentPage
{
	private readonly MainMenuViewModel _viewModel;
	private readonly IAppPreferences _appPreferences;
	private readonly INotifier _popupNotifier;

	public MainMenuPage(MainMenuViewModel viewModel, IAppPreferences appPreferences, INotifier popupNotifier)
	{
		InitializeComponent();
		
		// Hides the App bar at the top of the screen
        Shell.SetNavBarIsVisible(this, false);

        _viewModel = viewModel;
        BindingContext = viewModel;

		_appPreferences = appPreferences;
		_popupNotifier = popupNotifier;
    }

	protected async override void OnAppearing()
	{
		await _viewModel.LoadToursFromDatabase();

		if (_appPreferences.ContainsKey("lastLandmarkVisitedDate")
			&& _appPreferences.ContainsKey("lastLandmarkVisitedId"))
		{
			string date = _appPreferences.GetPreference("lastLandmarkVisitedDate", "");
			int id = _appPreferences.GetPreference("lastLandmarkVisitedId", 0);

			await _popupNotifier.ShowNotificationAsync(
				string.Format(AppResources.Landmark_Progress_Message, date, id),
				AppResources.Landmark_Progress_Title,
				"OK"
			);
		}
	}

	private async void TourButton_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.BindingContext is Tour tour)
			await _viewModel.StartTourAsync(tour);
    }

    private async void InstructionsButton_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("InstructionsPage");
    }
}