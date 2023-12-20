using VVVoyage.Models;

namespace VVVoyage.Database;

public interface IAppDatabase
{
    Task<Tour> GetRandomRouteAsync(string locale);
    Task<Tour> GetRouteAsync(string locale, string name);
}