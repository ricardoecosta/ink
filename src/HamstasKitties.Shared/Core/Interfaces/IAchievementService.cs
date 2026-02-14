namespace HamstasKitties.Core.Interfaces;

public interface IAchievementService : IManager
{
    void UnlockAchievement(string achievementId);
    IEnumerable<AchievementData> GetAchievements();
    AchievementData? GetAchievementById(string id);

    event EventHandler<AchievementUnlockedEventArgs>? OnAchievementUnlocked;
}

public class AchievementData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Reward { get; set; }
    public bool IsUnlocked { get; set; }
}

public class AchievementUnlockedEventArgs : EventArgs
{
    public AchievementData Achievement { get; set; } = null!;
}
