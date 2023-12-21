using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Database;
using VVVoyage.Models;

namespace VVVoyage.ViewModels
{

    public partial class MainMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Tour> _tours = [];

        private readonly IAppDatabase _database;
        private string _appLocale = "en";

        public MainMenuViewModel(IAppDatabase database)
        {
            _database = database;
        }

        public async Task LoadToursFromDatabase()
        {
            await _database.Init();

            string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (lang == "nl") _appLocale = "nl";
            Tours = new(await _database.GetAllToursAsync(_appLocale));

            Tours.Add(new(
                Resources.Localization.AppResources.Random_Tour_Name,
                Resources.Localization.AppResources.Random_Tour_Description,
                true,
                []
            ));
        }

        public async Task StartTourAsync(Tour tour)
        {
            Tour tourToStart;
            

            if (tour.IsRandomTour)
            {
                tourToStart = await _database.GetRandomRouteAsync(_appLocale);
            }
            else
            {
                tourToStart = tour;
            }

            Dictionary<string, object> parameters = new() {{ "Tour", tourToStart }, { "LandmarkStartIndex", 0 } };

            await Shell.Current.GoToAsync($"MainPage", parameters);
        }
    }
}
