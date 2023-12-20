using VVVoyage.Models;

namespace VVVoyage.Database;

public interface IAppDatabase
{
    Task Init();
    Task AddLandmarkAsync(Sight sight);
    Task AddTourAsync(Tour tour);
    Task AddDescriptionAsync(Sight sight, string locale, string description);
    Task AddDescriptionAsync(Tour tour, string locale, string description);
    Task<Tour> GetRandomRouteAsync(string locale);
    Task<Tour> GetRouteAsync(string locale, string name);
    Task<List<Tour>> GetAllToursAsync(string locale);
    Task Close();
}