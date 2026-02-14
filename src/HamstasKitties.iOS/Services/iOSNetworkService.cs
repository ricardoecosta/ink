using HamstasKitties.Core.Interfaces;
using Network;

namespace HamstasKitties.iOS.Services;

public class iOSNetworkService : INetworkService
{
    private readonly NWPathMonitor _monitor;
    private bool _isConnected;

    public iOSNetworkService()
    {
        _monitor = new NWPathMonitor();
        _monitor.PathUpdated += path =>
        {
            var wasConnected = _isConnected;
            _isConnected = path.Status == NWPathStatus.Satisfied;

            if (_isConnected && !wasConnected)
                OnNetworkOnline?.Invoke(this, EventArgs.Empty);
            else if (!_isConnected && wasConnected)
                OnNetworkOffline?.Invoke(this, EventArgs.Empty);
        };
    }

    public bool IsNetworkAvailable => _isConnected;

    public event EventHandler? OnNetworkOnline;
    public event EventHandler? OnNetworkOffline;

    public void Initialize()
    {
        _monitor.Start();
    }

    public void CheckConnection()
    {
        if (IsNetworkAvailable)
            OnNetworkOnline?.Invoke(this, EventArgs.Empty);
        else
            OnNetworkOffline?.Invoke(this, EventArgs.Empty);
    }
}
