namespace VVVoyage.Database;

public class AppPreferences : IAppPreferences
{
    private IPreferences _preferences = Preferences.Default;

    public AppPreferences(bool reset)
    {
        if (reset)
        {
            RemovePreference("lastLandmarkVisitedDate");
            RemovePreference("lastLandmarkVisitedId");
        }
    }

    public T GetPreference<T>(string key, T defaultValue)
    {
        return _preferences.Get(key, defaultValue);
    }

    public void SetPreference<T>(string key, T value)
    {
        _preferences.Set(key, value);
    }

    public void RemovePreference(string key)
    {
        _preferences.Remove(key);
    }

    public bool ContainsKey(string key)
    {
        return _preferences.ContainsKey(key);
    }
}
