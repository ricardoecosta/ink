using Foundation;
using UIKit;
using Microsoft.Xna.Framework;

namespace HamstasKitties.iOS;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    private Game1? _game;

    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        _game = new Game1();
        _game.Run();

        return true;
    }

    public override void OnResignActivation(UIApplication application)
    {
        // Pause game when app loses focus
    }

    public override void DidEnterBackground(UIApplication application)
    {
        // Save game state
    }

    public override void WillEnterForeground(UIApplication application)
    {
        // Restore game state
    }

    public override void OnActivated(UIApplication application)
    {
        // Resume game
    }
}

[Register("AppDelegate")]
class Program
{
    static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
