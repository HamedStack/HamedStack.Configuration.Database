using Microsoft.Extensions.Primitives;

namespace HamedStack.Configuration.Database;

/// <summary>
/// Provides an implementation of <see cref="IDatabaseConfigurationWatcher"/> to periodically
/// check for changes in the database configuration. This class uses a timer to trigger
/// change checks at a specified interval.
/// </summary>
/// <remarks>
/// This class is designed to monitor database configuration changes by periodically
/// invoking a provided delegate. It uses a <see cref="System.Threading.Timer"/> for scheduling
/// the checks and <see cref="System.Threading.CancellationTokenSource"/> for managing cancellation
/// of the monitoring process. It is important to dispose of instances of this class
/// when they are no longer needed to ensure that timer resources are properly released.
/// </remarks>
internal class DatabasePeriodicalWatcher : IDatabaseConfigurationWatcher
{
    private IChangeToken _changeToken = null!;
    private readonly Timer _timer;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasePeriodicalWatcher"/> class with a
    /// specified refresh interval.
    /// </summary>
    /// <param name="refreshInterval">The interval between checks for configuration changes.</param>
    public DatabasePeriodicalWatcher(TimeSpan refreshInterval)
    {
        _timer = new Timer(Change!, null, TimeSpan.Zero, refreshInterval);
    }

    /// <summary>
    /// Invoked by the timer to signal a potential change in the database configuration.
    /// </summary>
    /// <param name="state">The state object passed to the <see cref="Timer"/> callback. Not used in this implementation.</param>
    private void Change(object state)
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Starts monitoring for changes in the database configuration and returns an
    /// <see cref="IChangeToken"/> that can be used to subscribe to change notifications.
    /// </summary>
    /// <returns>An <see cref="IChangeToken"/> for subscribing to change notifications.</returns>
    public IChangeToken Watch()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);

        return _changeToken;
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="DatabasePeriodicalWatcher"/> class.
    /// </summary>
    /// <remarks>
    /// Calling <see cref="Dispose"/> allows the resources used by the <see cref="Timer"/> and
    /// <see cref="CancellationTokenSource"/> to be reallocated for other purposes. It should be called
    /// as soon as the watcher is no longer needed.
    /// </remarks>
    public void Dispose()
    {
        _timer.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}
