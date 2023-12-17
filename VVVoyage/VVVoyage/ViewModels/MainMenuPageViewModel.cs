using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.ViewModels
{
    public partial class MainMenuPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Tour> tours;

        public MainMenuPageViewModel()
        {
            tours = [
                new("Antique tour", "Discover some of the oldest landmarks of Breda. Find out what cultural significance they still have today."),
                new("Food tour", "Breda is home to some of the most unique snacks and meals. Ever wanted to know what a frikandel or a kroket tastes like?"),
                new("Wagenberg tour", "One-way ticket to Wagenberg (if it even exists). You'll probably die alone on this tour.")
            ];
        }
    }
}
