namespace HamstasKitties.Services.Firebase;

/// <summary>
/// Configuration for Firebase services.
/// Set these values from your Firebase Console.
/// </summary>
public static class FirebaseConfig
{
    // Android: Copy google-services.json to src/HamstasKitties.Android/Assets/
    // iOS: Copy GoogleService-Info.plist to src/HamstasKitties.iOS/

    /// <summary>
    /// Firebase Project ID (used for Firestore)
    /// </summary>
    public const string ProjectId = "YOUR_PROJECT_ID";

    /// <summary>
    /// Firebase API Key
    /// </summary>
    public const string ApiKey = "YOUR_API_KEY";

    /// <summary>
    /// Firebase App ID
    /// </summary>
    public const string AppId = "YOUR_APP_ID";

    /// <summary>
    /// Cloud Messaging Sender ID (for push notifications)
    /// </summary>
    public const string MessagingSenderId = "YOUR_SENDER_ID";

    /// <summary>
    /// Leaderboard collection name in Firestore
    /// </summary>
    public const string LeaderboardCollection = "leaderboards";

    /// <summary>
    /// User saves collection name in Firestore
    /// </summary>
    public const string SavesCollection = "user_saves";

    /// <summary>
    /// Achievements collection name in Firestore
    /// </summary>
    public const string AchievementsCollection = "user_achievements";
}
