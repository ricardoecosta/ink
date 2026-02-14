namespace HamstasKitties.Core.Interfaces;

public interface ILeaderboardService
{
    void Initialize(string gameId, string gameSecret);
    void SubmitScore(string leaderboardId, double score);
    IEnumerable<LeaderboardEntry> GetLeaderboard(string leaderboardId);

    event EventHandler<ScoreSubmittedEventArgs>? OnScoreSubmitted;
}

public class LeaderboardEntry
{
    public string Username { get; set; } = string.Empty;
    public double Score { get; set; }
    public int Rank { get; set; }
}

public class ScoreSubmittedEventArgs : EventArgs
{
    public string LeaderboardId { get; set; } = string.Empty;
    public double Score { get; set; }
    public bool Success { get; set; }
}
