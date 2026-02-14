#nullable disable

namespace HamstasKitties.Core.Interfaces;

public interface INetworkService
{
    bool IsNetworkAvailable { get; }

    void Initialize();
    void CheckConnection();

    event EventHandler OnNetworkOnline;
    event EventHandler OnNetworkOffline;
}
