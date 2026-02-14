using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Services.Firebase;

/// <summary>
/// Hybrid achievement service using both local storage and Firebase.
/// Replaces the old Scoreloop achievement system.
/// </summary>
public class HybridAchievementService : IAchievementService
{
    private readonly Dictionary<string, AchievementData> _achievements = new();
    private readonly ISettingsManager _settingsManager;
    private bool _isInitialized;

    public event EventHandler<AchievementUnlockedEventArgs>? OnAchievementUnlocked;

    public HybridAchievementService(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public bool Initialize()
    {
        LoadAchievements();
        _isInitialized = true;
        return true;
    }

    public bool Finalize()
    {
        SaveAchievements();
        return true;
    }

    public void UnlockAchievement(string achievementId)
    {
        if (_achievements.TryGetValue(achievementId, out var achievement))
        {
            if (achievement.IsUnlocked) return;

            achievement.IsUnlocked = true;
            achievement.UnlockedAt = DateTime.UtcNow;

            OnAchievementUnlocked?.Invoke(this, new AchievementUnlockedEventArgs
            {
                Achievement = achievement
            });

            // Sync to Firebase in production
            SyncAchievementToCloudAsync(achievement).ConfigureAwait(false);

            System.Diagnostics.Debug.WriteLine($"[Achievement] Unlocked: {achievement.Name}");
        }
    }

    public IEnumerable<AchievementData> GetAchievements()
    {
        return _achievements.Values;
    }

    public AchievementData? GetAchievementById(string id)
    {
        return _achievements.TryGetValue(id, out var achievement) ? achievement : null;
    }

    public void RegisterAchievement(AchievementData achievement)
    {
        _achievements[achievement.Id] = achievement;
    }

    private void LoadAchievements()
    {
        // Load from local settings
        if (_settingsManager.ContainsSetting("achievements"))
        {
            // In production, deserialize from settings
        }
    }

    private void SaveAchievements()
    {
        // Save to local settings
        _settingsManager.SaveSetting("achievements", _achievements.Values.ToList());
    }

    private async Task SyncAchievementToCloudAsync(AchievementData achievement)
    {
        // In production:
        // var db = FirebaseFirestore.Instance;
        // var userId = FirebaseAuth.Instance.CurrentUser?.Uid;
        // if (userId == null) return;
        //
        // var docRef = db.Collection("users").Document(userId).Collection("achievements").Document(achievement.Id);
        // await docRef.SetAsync(achievement);

        await Task.CompletedTask;
    }

    public async Task SyncFromCloudAsync()
    {
        // Download achievements from Firebase and merge with local
        await Task.CompletedTask;
    }
}
