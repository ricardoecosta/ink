#nullable disable
#nullable disable

using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Services.Firebase;

/// <summary>
/// Firebase Analytics implementation replacing Flurry Analytics.
/// Requires Firebase SDK to be configured in platform projects.
/// </summary>
public class FirebaseAnalyticsService : IAnalyticsService
{
    private bool _isInitialized;
    private string _apiKey;
    private string _appVersion;

    public bool IsInitialized => _isInitialized;

    public void Initialize(string apiKey, string appVersion)
    {
        _apiKey = apiKey;
        _appVersion = appVersion;
        _isInitialized = true;

        // Platform-specific Firebase initialization should be done in MainActivity/AppDelegate
        // This service assumes FirebaseApp.InitializeApp() has been called
    }

    public void LogError(Exception exception)
    {
        if (!_isInitialized) return;

        // In production, use Firebase Crashlytics
        // Firebase.Crashlytics.Crashlytics.Log(exception.Message);
        System.Diagnostics.Debug.WriteLine($"[Analytics] Error: {exception.Message}");
    }

    public void LogEvent(string eventName, params (string Name, string Value)[] parameters)
    {
        if (!_isInitialized) return;

        // In production, use Firebase Analytics
        // var bundle = new Bundle();
        // foreach (var (name, value) in parameters)
        //     bundle.PutString(name, value);
        // FirebaseAnalytics.GetInstance(context).LogEvent(eventName, bundle);

        System.Diagnostics.Debug.WriteLine($"[Analytics] Event: {eventName}, Params: {string.Join(", ", parameters.Select(p => $"{p.Name}={p.Value}"))}");
    }
}
