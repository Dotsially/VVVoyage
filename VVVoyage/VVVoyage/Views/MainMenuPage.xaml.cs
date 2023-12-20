using System.Diagnostics;
using VVVoyage.Database;
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

	protected async override void OnAppearing()
	{
		await _viewModel.LoadToursFromDatabase();
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