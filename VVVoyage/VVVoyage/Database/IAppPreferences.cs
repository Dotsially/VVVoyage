namespace VVVoyage.Database;

public interface IAppPreferences
{
    /// <summary>
    /// Gets the value for the given key.
    /// </summary>
    /// <param name="key">Key to look up</param>
    /// <param name="defaultValue">Value to return if there is no existing value</param>
    /// <typeparam name="T">Type of value</typeparam>
    /// <returns>The value found.</returns>
    T GetPreference<T>(string key, T defaultValue);

    /// <summary>
    /// Sets the value for the given key.
    /// </summary>
    /// <param name="key">Key to set value for</param>
    /// <param name="value">Value to set</param>
    /// <typeparam name="T">Type of value</typeparam>
    void SetPreference<T>(string key, T value);

    /// <summary>
    /// Remove value for the given key.
    /// </summary>
    /// <param name="key">Key to remove value for</param>
    void RemovePreference(string key);

    /// <summary>
    /// Check if the preferences contain a value for the given key.
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True if key is found otherwise false</returns>
    bool ContainsKey(string key);
}
