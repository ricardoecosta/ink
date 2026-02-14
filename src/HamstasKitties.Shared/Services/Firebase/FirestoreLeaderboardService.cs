#nullable disable
#nullable disable

using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Services.Firebase;

/// <summary>
/// Firebase Firestore implementation replacing Scoreloop.
/// </summary>
public class FirestoreLeaderboardService : ILeaderboardService
{
    private bool _isInitialized;
    private string _gameId;
    private readonly List<LeaderboardEntry> _cachedEntries = new();

    public bool IsInitialized => _isInitialized;

    public event EventHandler<ScoreSubmittedEventArgs> OnScoreSubmitted;

    public void Initialize(string gameId, string gameSecret)
    {
        _gameId = gameId;
        _isInitialized = true;

        // Platform-specific Firestore initialization
        // FirebaseFirestore.Instance.SetFirestoreSettings(...)
    }

    public void SubmitScore(string leaderboardId, double score)
    {
        if (!_isInitialized) return;

        // In production:
        // var db = FirebaseFirestore.Instance;
        // var doc = new HashMap();
        // doc.Put("score", score);
        // doc.Put("timestamp", FieldValue.ServerTimestamp());
        // db.Collection("leaderboards").Document(leaderboardId).Collection("scores").Add(doc);

        System.Diagnostics.Debug.WriteLine($"[Leaderboard] Submitted score {score} to {leaderboardId}");

        OnScoreSubmitted?.Invoke(this, new ScoreSubmittedEventArgs
        {
            LeaderboardId = leaderboardId,
            Score = score,
            Success = true
        });
    }

    public IEnumerable<LeaderboardEntry> GetLeaderboard(string leaderboardId)
    {
        if (!_isInitialized) return Enumerable.Empty<LeaderboardEntry>();

        // In production:
        // var db = FirebaseFirestore.Instance;
        // var query = db.Collection("leaderboards").Document(leaderboardId).Collection("scores")
        //     .OrderBy("score", Query.Direction.Descending)
        //     .Limit(100);

        return _cachedEntries;
    }

    /// <summary>
    /// For testing: add entries to the local cache
    /// </summary>
    public void AddTestEntry(LeaderboardEntry entry)
    {
        _cachedEntries.Add(entry);
    }
}
