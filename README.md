# Library Overview

This library enables applications to dynamically load and update configurations from a relational database. It supports automatic refresh intervals and manual reloading of settings, ensuring that your application can adapt to configuration changes on-the-fly without restarting or redeploying the application. 

## Features

- **Dynamic Configuration Loading:** Load key/value pairs from a relational database to manage application settings.
- **Automatic Refresh:** Configure a reload timer to refresh settings automatically at specified intervals.
- **Manual Reload Option:** Manually trigger a reload of settings to ensure the application can quickly adapt to configuration changes.
- **Customizable Schema:** Flexibly define the database schema, table, and column names to fit your existing database structure.

## Preparing Your Database

First, ensure your database includes a table for storing configuration key/value pairs. The default expected table structure is as follows:

```sql
CREATE TABLE Settings (
    Key VARCHAR(255) NOT NULL,
    Value VARCHAR(255),
    PRIMARY KEY (Key)
);
```

## Library Setup

### Basic Configuration

1. **Establish Database Connection:** Begin by creating a `DbConnection` instance using your database connection string. This connection will be used to load settings from the database.

   ```csharp
   var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
   var dbConnection = new SqlConnection(connectionString);
   ```

2. **Add Database Configuration to Your Application:** Integrate the database settings into your application's configuration system.

   ```csharp
   builder.Configuration.AddDatabase(dbConnection);
   ```

### Advanced Configuration

For advanced scenarios, you can customize the default behavior by specifying additional options:

```csharp
builder.Configuration.AddDatabase(dbConnection, option =>
{
    option.Schema = "cfg"; // Default is empty.
    option.Table = "Configurations"; // Default is "Settings".
    option.KeyColumn = "K"; // Default is "Key".
    option.ValueColumn = "V"; // Default is "Value".
    option.AutoReload = new TimeSpan(0, 0, 0, 20); // Set auto-reload interval for every 20 seconds. Default is no auto reload (zero).
});
```

## Using Configuration in Your Application

### Register Configuration Classes

1. **Define Configuration POCO:** Create a class representing your configuration settings, mapping each property to a specific configuration key.

   ```csharp
   public sealed class Site
   {
       public string Name { get; init; } // Maps to Site:Name in the database
   }
   ```

2. **Register and Set in DI:** Register your configuration class with the Dependency Injection (DI) container and set it up for use within your application.

   ```csharp
   builder.Services.Configure<Site>(builder.Configuration.GetSection("Site"));
   ```

### Accessing Configurations

- Utilize `IOptionsSnapshot` to access the latest configuration values within your application components, ensuring you can react to changes in configuration dynamically.

   ```csharp
   public class WeatherForecastController : ControllerBase
   {
       private readonly IOptionsSnapshot<Site> _settingsSnapshot;

       public WeatherForecastController(IOptionsSnapshot<Site> settingsSnapshot)
       {
           _settingsSnapshot = settingsSnapshot;
       }
   }
   ```

- Access configuration values through the `_settingsSnapshot` instance. For example, if your database has a record with `Key: Site:Name` and `Value: www.google.com`, you can retrieve this value using `_settingsSnapshot.Value.Name`.

## Refreshing Configuration

- **Automatic Refresh:** If `AutoReload` is set, the library will automatically refresh configuration values from the database at the specified intervals.
- **Manual Refresh:** Call `DatabaseConfigurationSource.Reload();` to manually trigger a configuration reload at any time.
