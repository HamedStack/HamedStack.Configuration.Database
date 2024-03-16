using Microsoft.Extensions.Primitives;

namespace HamedStack.Configuration.Database;

/// <summary>
/// Defines the contract for watching database configuration changes. Implementing
/// classes can monitor configuration changes and provide notifications when such changes occur.
/// </summary>
/// <remarks>
/// This interface is designed for use in applications that require dynamic response
/// to changes in database configuration settings. Implementations should ensure efficient
/// resource management, especially for long-running applications, by implementing
/// the <see cref="System.IDisposable"/> interface to release resources when no longer needed.
/// </remarks>
internal interface IDatabaseConfigurationWatcher : IDisposable
{
    /// <summary>
    /// Starts watching for changes in the database configuration.
    /// </summary>
    /// <returns>
    /// An <see cref="Microsoft.Extensions.Primitives.IChangeToken"/> that can be used to subscribe to
    /// change notifications. Subscribers can register callback methods to be invoked when a change is detected.
    /// </returns>
    IChangeToken Watch();
}
