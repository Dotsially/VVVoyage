namespace VVVoyage
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Button button = new Button();
            button.Clicked += async (sender, args) =>
            {
                await Navigation.PushAsync(new RoutePage());
            };
            Content = button;
        }
    }

}
