using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core.Mocks;

public class MockNetworkService : INetworkService
{
    public bool IsNetworkAvailable { get; set; } = true;
    public int CheckConnectionCallCount { get; private set; }

    public event EventHandler? OnNetworkOnline;
    public event EventHandler? OnNetworkOffline;

    public void Initialize() { }
    public void CheckConnection() => CheckConnectionCallCount++;

    public void SimulateOnline() => OnNetworkOnline?.Invoke(this, EventArgs.Empty);
    public void SimulateOffline() => OnNetworkOffline?.Invoke(this, EventArgs.Empty);
}
