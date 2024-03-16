using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace HamedStack.Configuration.Database;

/// <summary>
/// Provides a configuration provider that loads settings from a database. This provider supports
/// dynamic reloading of configuration settings if a <see cref="IDatabaseConfigurationWatcher"/> is
/// provided in the <see cref="DatabaseConfigurationSource"/>.
/// </summary>
/// <remarks>
/// The <see cref="DatabaseConfigurationProvider"/> queries a specified database table to load
/// configuration key-value pairs into the application's configuration. It listens for change notifications
/// via an <see cref="IDatabaseConfigurationWatcher"/>, if available, to reload the configuration automatically
/// upon changes. This provider implements <see cref="IDisposable"/> to ensure proper disposal of
/// database connections and change token registrations.
/// </remarks>
public class DatabaseConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly DatabaseConfigurationSource _source;
    private readonly IDisposable _changeTokenRegistration = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseConfigurationProvider"/> class with the
    /// specified source.
    /// </summary>
    /// <param name="source">The <see cref="DatabaseConfigurationSource"/> containing the database connection
    /// and table information for loading configuration settings.</param>
    public DatabaseConfigurationProvider(DatabaseConfigurationSource source)
    {
        _source = source;
        if (_source.DatabaseConfigurationWatcher != null)
        {
            _changeTokenRegistration = ChangeToken.OnChange(
                () => _source.DatabaseConfigurationWatcher.Watch(),
                Load
            );
        }
    }

    /// <summary>
    /// Loads configuration key-value pairs from the database into the provider's data store.
    /// </summary>
    /// <remarks>
    /// This method queries the database using the connection and table information provided in the
    /// <see cref="DatabaseConfigurationSource"/>. It handles database connection state checks and
    /// opens the connection if necessary. Any errors during the load operation are caught and ignored,
    /// allowing the application to continue without crashing.
    /// </remarks>
    public override void Load()
    {
        try
        {
            if (_source.DbConnection.State != ConnectionState.Open)
                _source.DbConnection.Open();

            var query = !string.IsNullOrWhiteSpace(_source.Schema)
                ? $"SELECT [{_source.KeyColumn}], [{_source.ValueColumn}] FROM [{_source.Schema}].[{_source.Table}]"
                : $"SELECT [{_source.KeyColumn}], [{_source.ValueColumn}] FROM [{_source.Table}]";

            var data = _source
                .DbConnection
                .Query<KeyValuePair<string, string>>(query)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            Data = data!;
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Triggers a reload of the configuration data from the database.
    /// </summary>
    public void Reload()
    {
        Load();
    }

    /// <summary>
    /// Disposes of resources used by the provider, including database connections and change token registrations.
    /// </summary>
    /// <remarks>
    /// Ensures the database connection is closed if it's open, and disposes of any <see cref="IDisposable"/>
    /// resources, including the change token registration and potentially the database configuration watcher.
    /// </remarks>
    public void Dispose()
    {
        _changeTokenRegistration.Dispose();
        _source.DatabaseConfigurationWatcher?.Dispose();
        if (_source.DbConnection.State == ConnectionState.Open)
            _source.DbConnection.Close();
    }
}
