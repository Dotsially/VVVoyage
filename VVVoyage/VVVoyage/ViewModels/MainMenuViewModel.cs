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
            new("Old VVV building", new Location(51.592412225120796, 4.775692730162895), "The building in which the VVV used to be housed inside."),
            new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), "Description of the Liefdeszusters landmark."),
            new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), "Description of the Nassau Baronie landmark."),
            new("The Light House", new Location(51.584039168945026, 4.774673039583854), "Description of the light house."),
            new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), "Description of the Castle of Breda."),
        ];

        private static readonly List<Sight> tour2Landmarks =
        [
            new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), "Description of the Nassau Baronie landmark."),
            new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), "Description of the Castle of Breda."),
            new("Old VVV building", new Location(51.592412225120796, 4.775692730162895), "The building in which the VVV used to be housed inside."),
            new("The Light House", new Location(51.584039168945026, 4.774673039583854), "Description of the light house."),
            new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), "Description of the Liefdeszusters landmark."),
        ];

        private static readonly List<Sight> tour3Landmarks =
        [
            new("Liefdeszusters", new Location(51.59336561016905, 4.779405797254084), "Description of the Liefdeszusters landmark."),
            new("Nassau Baronie Monument", new Location(51.59268164269348, 4.779718410749389), "Description of the Nassau Baronie landmark."),
            new("The Light House", new Location(51.584039168945026, 4.774673039583854), "Description of the light house."),
            new("Kasteel van Breda", new Location(51.59108157152743, 4.776103712549693), "Description of the Castle of Breda."),
            new("Old VVV building", new Location(51.592412225120796, 4.775692730162895), "The building in which the VVV used to be housed inside."),
        ];

        [ObservableProperty]
        public ObservableCollection<Tour> tours = [
            new(
                "Antique tour",
                "Discover some of the oldest landmarks of Breda. Find out what cultural significance they still have today.",
                [.. tour1Landmarks]
            ),
            new(
                "Food tour",
                "Breda is home to some of the most unique snacks and meals. Ever wanted to know what a frikandel or a kroket tastes like?",
                [.. tour2Landmarks]
            ),
            new(
                "Wagenberg tour",
                "One-way ticket to Wagenberg (if it even exists). You'll probably die alone on this tour.",
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
