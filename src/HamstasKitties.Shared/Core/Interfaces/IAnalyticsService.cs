namespace HamstasKitties.Core.Interfaces;

public interface IAnalyticsService
{
    void Initialize(string apiKey, string appVersion);
    void LogError(Exception exception);
    void LogEvent(string eventName, params (string Name, string Value)[] parameters);
}
