using System.Data;
using Microsoft.Extensions.Configuration;

namespace HamedStack.Configuration.Database;

/// <summary>
/// Provides extension methods to <see cref="IConfigurationBuilder"/> for adding database-based configuration sources.
/// </summary>
public static class DatabaseConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds a database configuration source to the <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add the database configuration to.</param>
    /// <param name="dbConnection">The database connection to use for reading configuration data.</param>
    /// <param name="configureOptions">An action to configure the <see cref="DatabaseConfigurationOption"/> for accessing database configuration.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/> with the database configuration source added.</returns>
    /// <remarks>
    /// This method allows for configuring the application settings to be loaded from a database. 
    /// The <paramref name="dbConnection"/> parameter specifies the database connection to use. 
    /// The <paramref name="configureOptions"/> action provides a way to specify additional details such as 
    /// the database schema, table, and columns for key and value, as well as an auto-reload interval.
    /// 
    /// Example usage:
    /// <code>
    /// var builder = new ConfigurationBuilder()
    ///     .AddDatabase(connection, opts => {
    ///         opts.Schema = "AppConfig";
    ///         opts.Table = "Settings";
    ///         opts.KeyColumn = "Key";
    ///         opts.ValueColumn = "Value";
    ///         opts.AutoReload = TimeSpan.FromMinutes(5);
    ///     });
    /// </code>
    /// </remarks>
    public static IConfigurationBuilder AddDatabase(this IConfigurationBuilder builder, IDbConnection dbConnection, Action<DatabaseConfigurationOption>? configureOptions)
    {
        var options = new DatabaseConfigurationOption();
        configureOptions?.Invoke(options);
        var databaseConfigurationSource = new DatabaseConfigurationSource
        {
            DbConnection = dbConnection,
            KeyColumn = options.KeyColumn,
            Schema = options.Schema,
            ValueColumn = options.ValueColumn,
            Table = options.Table,
            DatabaseConfigurationWatcher = options.AutoReload > TimeSpan.Zero ? new DatabasePeriodicalWatcher(options.AutoReload) : null
        };
        return builder.Add(databaseConfigurationSource);
    }
}
