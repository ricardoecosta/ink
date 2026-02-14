using System;

namespace HamstasKitties.Utils;

/// <summary>
/// Stub for GuideHelper (from XNA GamerServices) which is not available in MonoGame.
/// This provides compatibility for code that was written for XNA.
/// </summary>
public static class GuideHelper
{
    /// <summary>
    /// Stub - shows a synchronous OK button alert. Returns null in MonoGame.
    /// </summary>
    public static int? ShowSyncOkButtonAlertMsgBox(string title, string message)
    {
        // Stub implementation - in a real MonoGame app, you'd use a platform-specific dialog
        Console.WriteLine($"[ALERT] {title}: {message}");
        return null;
    }

    /// <summary>
    /// Stub - shows a synchronous Yes/No button alert. Returns null in MonoGame.
    /// </summary>
    public static int? ShowSyncYesNoButtonAlertMsgBox(string title, string message)
    {
        // Stub implementation - in a real MonoGame app, you'd use a platform-specific dialog
        Console.WriteLine($"[CONFIRM] {title}: {message}");
        return null;
    }

    /// <summary>
    /// Stub - shows keyboard input dialog. Returns empty string in MonoGame.
    /// </summary>
    public static string ShowKeyboardInput(string title, string description, string defaultText = "")
    {
        // Stub implementation - in a real MonoGame app, you'd use a platform-specific input dialog
        Console.WriteLine($"[INPUT] {title}: {description} (default: {defaultText})");
        return defaultText;
    }
}

/// <summary>
/// Stub for Guide (from XNA GamerServices) which is not available in MonoGame.
/// </summary>
public static class Guide
{
    /// <summary>
    /// Stub - always returns false in MonoGame (not in trial mode).
    /// </summary>
    public static bool IsTrialMode => false;

    /// <summary>
    /// Stub - simulated trial mode (always false in MonoGame).
    /// </summary>
    public static bool SimulateTrialMode => false;

    /// <summary>
    /// Stub - shows marketplace (does nothing in MonoGame).
    /// </summary>
    public static void ShowMarketplace(PlayerIndex playerIndex = PlayerIndex.One)
    {
        // Stub implementation - does nothing in MonoGame
        Console.WriteLine("[MARKETPLACE] ShowMarketplace called (not implemented in MonoGame)");
    }
}

/// <summary>
/// Stub for PlayerIndex enum (from XNA Framework).
/// </summary>
public enum PlayerIndex
{
    One,
    Two,
    Three,
    Four
}
