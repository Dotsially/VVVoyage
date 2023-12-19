namespace VVVoyage.Views;

public partial class InstructionsPage : ContentPage
{
	public InstructionsPage()
	{
		InitializeComponent();

        // Hides the App bar at the top of the screen
        Shell.SetNavBarIsVisible(this, false);
    }

	public async void BackBtn_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}