using System.Data;
using Microsoft.Extensions.Configuration;

namespace HamedStack.Configuration.Database;

/// <summary>
/// Represents a database-based configuration source for an application. This configuration source
/// allows settings to be loaded from a database table, facilitating dynamic configuration changes.
/// </summary>
/// <remarks>
/// The <see cref="DatabaseConfigurationSource"/> uses a specified database connection to load configuration
/// settings from a database. It requires the specification of a schema, table, and columns where the
/// configuration keys and values are stored. This class supports dynamic configuration updates by
/// optionally integrating with an <see cref="IDatabaseConfigurationWatcher"/> to monitor for changes in
/// the configuration data.
/// </remarks>
public class DatabaseConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// Gets or sets the database connection used to load configuration settings.
    /// </summary>
    public IDbConnection DbConnection { get; set; } = null!;

    /// <summary>
    /// Gets or sets the database schema where the configuration table is located.
    /// </summary>
    public string Schema { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the table containing configuration settings.
    /// </summary>
    public string Table { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the column containing configuration keys.
    /// </summary>
    public string KeyColumn { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the column containing configuration values.
    /// </summary>
    public string ValueColumn { get; set; } = null!;

    /// <summary>
    /// Optionally gets or sets the <see cref="IDatabaseConfigurationWatcher"/> used to monitor
    /// for changes in the database configuration.
    /// </summary>
    internal IDatabaseConfigurationWatcher? DatabaseConfigurationWatcher { get; set; }

    private static DatabaseConfigurationProvider? _provider;

    /// <summary>
    /// Builds the database configuration provider for this source.
    /// </summary>
    /// <param name="builder">The configuration builder that this source is attached to.</param>
    /// <returns>An <see cref="IConfigurationProvider"/> that loads configuration settings from the database.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        _provider = new DatabaseConfigurationProvider(this);
        return _provider;
    }

    /// <summary>
    /// Triggers a reload of the configuration settings from the database.
    /// </summary>
    /// <remarks>
    /// This static method allows for an application-wide refresh of the configuration settings
    /// by signaling the <see cref="DatabaseConfigurationProvider"/> to reload its data. This is
    /// particularly useful in scenarios where the configuration data has changed and the application
    /// needs to apply the new settings immediately.
    /// </remarks>
    public static void Reload()
    {
        _provider?.Reload();
    }
}
