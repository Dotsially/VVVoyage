using System.Diagnostics;
using VVVoyage.Models;
using VVVoyage.ViewModels;

namespace VVVoyage.Views;

public partial class MainMenuPage : ContentPage
{
	public MainMenuPage(MainMenuPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

	private void SelectedLanguage_Changed(object sender, EventArgs e)
	{
		if (sender is Picker picker && picker.SelectedItem is string selectedLanguage)
		{
			// TODO change language to newly selected language
			Debug.WriteLine(selectedLanguage);
		}
	}

	private void TourButton_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.BindingContext is Tour tour)
		{
			// TODO hook this up to MainMenuViewModel
			Debug.WriteLine($"{tour.Name}, {tour.Description}");
		}
	}
}