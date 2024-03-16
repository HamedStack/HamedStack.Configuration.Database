namespace HamedStack.Configuration.Database;

/// <summary>
/// Holds options for configuring database-based application settings.
/// </summary>
/// <remarks>
/// This class provides configuration options for specifying how to access and retrieve
/// application settings stored in a database. It includes details such as the database schema,
/// table name, and specific columns for the key-value pairs representing the settings.
/// Additionally, it supports setting an interval for automatic reloading of the configuration data.
/// </remarks>
public class DatabaseConfigurationOption
{
    /// <summary>
    /// Gets or sets the database schema where the configuration table is located.
    /// Default is an empty string, indicating the default schema.
    /// </summary>
    public string Schema { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the table containing configuration settings.
    /// Default is "Settings".
    /// </summary>
    public string Table { get; set; } = "Settings";

    /// <summary>
    /// Gets or sets the name of the column containing configuration keys.
    /// Default is "Key".
    /// </summary>
    public string KeyColumn { get; set; } = "Key";

    /// <summary>
    /// Gets or sets the name of the column containing configuration values.
    /// Default is "Value".
    /// </summary>
    public string ValueColumn { get; set; } = "Value";

    /// <summary>
    /// Gets or sets the interval for automatic reloading of configuration data from the database.
    /// The default is zero, indicating that auto-reloading is disabled. A non-zero value specifies
    /// the time interval between each reload.
    /// </summary>
    public TimeSpan AutoReload { get; set; } = new(0, 0, 0, 0);
}
