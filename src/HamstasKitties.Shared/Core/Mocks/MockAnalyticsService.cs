using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core.Mocks;

public class MockAnalyticsService : IAnalyticsService
{
    public List<Exception> LoggedErrors { get; } = new();
    public List<(string Name, (string, string)[] Params)> LoggedEvents { get; } = new();

    public bool IsInitialized { get; private set; }

    public void Initialize(string apiKey, string appVersion) => IsInitialized = true;

    public void LogError(Exception exception) => LoggedErrors.Add(exception);

    public void LogEvent(string eventName, params (string Name, string Value)[] parameters)
        => LoggedEvents.Add((eventName, parameters));
}
