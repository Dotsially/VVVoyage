using System.Diagnostics;

namespace VVVoyage.Views;

[QueryProperty(nameof(LandmarkStartIndex), "LandmarkStartIndex")]
public partial class InstructionsPage : ContentPage
{
    private int _landmarkStartIndex;
    public int LandmarkStartIndex
    {
        get => _landmarkStartIndex;
        set
        {
            _landmarkStartIndex = value;
            OnPropertyChanged();
        }
    }

    public InstructionsPage()
	{
		InitializeComponent();

        // Hides the App bar at the top of the screen
        Shell.SetNavBarIsVisible(this, false);
    }

	public async void BackBtn_Clicked(object sender, EventArgs e)
	{
        Debug.WriteLine($"On Instructions, landmark start index is: {LandmarkStartIndex}");

        Dictionary<string, object> param = new() { { "LandmarkStartIndex", LandmarkStartIndex } };

		await Shell.Current.GoToAsync("..", param);
	}
}