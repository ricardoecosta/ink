#if !DISABLE_NETWORKING
using System;
using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Management;

/// <summary>
/// Manages network connectivity status and notifications.
/// </summary>
public sealed class NetworkManager
{
    private static readonly Lazy<NetworkManager> _instance = new Lazy<NetworkManager>(() => new NetworkManager());
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private readonly INetworkService _networkService;
#pragma warning restore CS0414

    private NetworkManager()
    {
        _networkService = null; // Will be injected via Initialize()
    }

    public static NetworkManager Instance => _instance.Value;

    /// <summary>
    /// Initializes the network manager with a network service implementation.
    /// </summary>
    public void Initialize(INetworkService networkService)
    {
        if (networkService == null)
        {
            throw new ArgumentNullException(nameof(networkService));
        }

        // Note: For simplicity in this migration, we're not storing the service
        // The actual implementation would wire up events here
        networkService.Initialize();
    }

    /// <summary>
    /// Checks internet connection availability.
    /// </summary>
    public void CheckInternetConnection(bool forceCheck)
    {
        // Stub implementation - would call the actual service
    }

    /// <summary>
    /// Gets whether network is currently available.
    /// </summary>
    public bool IsNetworkAvailable => true; // Stub - always return true for now

    /// <summary>
    /// Event raised when network comes online.
    /// </summary>
#pragma warning disable CS0067 // Event is never used
    public event EventHandler OnNetworkOnline;

    /// <summary>
    /// Event raised when network goes offline.
    /// </summary>
    public event EventHandler OnNetworkOffline;
#pragma warning restore CS0067
}
#else
using System;

namespace HamstasKitties.Management;

/// <summary>
/// Stub NetworkManager for builds with networking disabled.
/// </summary>
public sealed class NetworkManager
{
    private static readonly Lazy<NetworkManager> _instance = new Lazy<NetworkManager>(() => new NetworkManager());

    private NetworkManager() { }

    public static NetworkManager Instance => _instance.Value;

    public void CheckInternetConnection(bool forceCheck) { }

    public bool IsNetworkAvailable => false;

    public event EventHandler OnNetworkOnline;
    public event EventHandler OnNetworkOffline;
}
#endif
