using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.ViewModels
{
    public partial class MainMenuViewModel : ObservableObject
    {
        private static readonly List<Sight> tour1Landmarks =
        [
            new("Old VVV building", new Location(51.594112, 4.779417), Resources.Localization.AppResources.Old_VVV_Description),
            new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), Resources.Localization.AppResources.Liefdeszusters_Description),
            new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), Resources.Localization.AppResources.Nassau_Description),
            new("The Light House", new Location(51.584039168945026, 4.774673039583854), Resources.Localization.AppResources.Light_House_Description),
            new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), Resources.Localization.AppResources.Castle_Description),
        ];

        private static readonly List<Sight> tour2Landmarks =
        [
            new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), Resources.Localization.AppResources.Nassau_Description),
            new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), Resources.Localization.AppResources.Castle_Description),
            new("Old VVV building", new Location(51.594112, 4.779417), Resources.Localization.AppResources.Old_VVV_Description),
            new("The Light House", new Location(51.584039168945026, 4.774673039583854), Resources.Localization.AppResources.Light_House_Description),
            new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), Resources.Localization.AppResources.Liefdeszusters_Description),
        ];

        private static readonly List<Sight> tour3Landmarks =
        [
            new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), Resources.Localization.AppResources.Liefdeszusters_Description),
            new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), Resources.Localization.AppResources.Nassau_Description),
            new("The Light House", new Location(51.584039168945026, 4.774673039583854), Resources.Localization.AppResources.Light_House_Description),
            new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), Resources.Localization.AppResources.Castle_Description),
            new("Old VVV building", new Location(51.594112, 4.779417), Resources.Localization.AppResources.Old_VVV_Description),
        ];

        [ObservableProperty]
        public ObservableCollection<Tour> tours = [
            new(
                "Antique tour",
                Resources.Localization.AppResources.Antique_Description,
                [.. tour1Landmarks]
            ),
            new(
                "Food tour",
                Resources.Localization.AppResources.Food_Description,
                [.. tour2Landmarks]
            ),
            new(
                "Wagenberg tour",
                Resources.Localization.AppResources.Wagenberg_Description,
                [.. tour3Landmarks]
            )
        ];

        public void SetLanguage(string language)
        {
            throw new NotImplementedException();
        }

        public async Task StartTourAsync(Tour tour)
        {
            Dictionary<string, object> param = new()
            {
                { "Tour", tour }
            };
            await Shell.Current.GoToAsync($"MainPage", param);
        }
    }
}
