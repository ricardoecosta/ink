using Android.Content;
using Android.Net;
using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Android.Services;

public class AndroidNetworkService : INetworkService
{
    private readonly ConnectivityManager? _connectivityManager;

    public AndroidNetworkService(Context context)
    {
        _connectivityManager = context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
    }

    public bool IsNetworkAvailable
    {
        get
        {
            if (_connectivityManager == null)
                return false;

            var activeNetwork = _connectivityManager.ActiveNetworkInfo;
            return activeNetwork != null && activeNetwork.IsConnected;
        }
    }

    public event EventHandler? OnNetworkOnline;
    public event EventHandler? OnNetworkOffline;

    public void Initialize()
    {
        // Register for network callbacks if needed
    }

    public void CheckConnection()
    {
        if (IsNetworkAvailable)
            OnNetworkOnline?.Invoke(this, EventArgs.Empty);
        else
            OnNetworkOffline?.Invoke(this, EventArgs.Empty);
    }
}
