namespace VVVoyage.Views;

public partial class InstructionsPage : ContentPage
{
	public InstructionsPage()
	{
		InitializeComponent();
	}

	public async void BackBtn_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}