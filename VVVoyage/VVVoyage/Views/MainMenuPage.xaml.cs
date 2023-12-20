using System.Diagnostics;
using VVVoyage.Models;
using VVVoyage.ViewModels;

namespace VVVoyage.Views;

public partial class MainMenuPage : ContentPage
{
	private readonly MainMenuViewModel _viewModel;

	public MainMenuPage(MainMenuViewModel viewModel)
	{
		InitializeComponent();
		
		// Hides the App bar at the top of the screen
        Shell.SetNavBarIsVisible(this, false);

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

	private void SelectedLanguage_Changed(object sender, EventArgs e)
	{
		if (sender is Picker picker && picker.SelectedItem is string selectedLanguage)
			_viewModel.SetLanguage(selectedLanguage);
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