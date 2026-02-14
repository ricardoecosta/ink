using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Xna.Framework;

namespace HamstasKitties.Android;

[Activity(
    Label = "Hamstas'n'Kitties",
    MainLauncher = true,
    Icon = "@drawable/icon",
    Theme = "@android:style/Theme.NoTitleBar",
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ScreenOrientation = ScreenOrientation.Portrait,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
public class MainActivity : AndroidGameActivity
{
    private Game1? _game;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _game = new Game1();
        SetContentView(_game.Services.GetService(typeof(View)) as View ?? new View(this));
        _game.Run();
    }

    protected override void OnPause()
    {
        base.OnPause();
        _game?.SuppressDraw();
    }

    protected override void OnResume()
    {
        base.OnResume();
        // Game will resume drawing automatically
    }
}
