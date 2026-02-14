# Hamstas'n'Kitties

A match-3 puzzle game ported from Windows Phone/XNA to modern .NET 8 and MonoGame.

## About

Hamstas'n'Kitties is a casual match-3 puzzle game featuring adorable hamsters and kittens. Match blocks of the same type to clear them and earn points, with special power-ups like bombs and magic abilities adding strategic depth.

## Platforms

- **Desktop** - Windows, macOS, Linux (DesktopGL)
- **Android** - API 24+
- **iOS** - iOS 12.0+

## Technology Stack

- **.NET 8** - Modern C# and runtime
- **MonoGame 3.8** - Cross-platform game framework
- **xUnit + FluentAssertions** - Unit testing
- **GitHub Actions** - CI/CD

## Project Structure

```
src/
├── HamstasKitties.Shared/     # Shared game code
│   ├── Core/                   # Core framework, managers
│   ├── Mechanics/              # Game mechanics (Block, Matching)
│   ├── Scenes/                 # Game scenes
│   ├── Layers/                 # UI layers
│   ├── Services/               # Firebase services
│   └── UI/                     # UI components
├── HamstasKitties.Desktop/    # Desktop platform
├── HamstasKitties.Android/    # Android platform
└── HamstasKitties.iOS/        # iOS platform

tests/
└── HamstasKitties.Shared.Tests/  # Unit tests
```

## Building

### Prerequisites

- .NET 8 SDK
- MonoGame Extension (for content pipeline)
- Android SDK (for Android builds)
- Xcode (for iOS builds, macOS only)

### Build Commands

```bash
# Build shared library
dotnet build src/HamstasKitties.Shared/HamstasKitties.Shared.csproj

# Build desktop version
dotnet build src/HamstasKitties.Desktop/HamstasKitties.Desktop.csproj

# Build Android version
dotnet build src/HamstasKitties.Android/HamstasKitties.Android.csproj

# Run tests
dotnet test tests/HamstasKitties.Shared.Tests/HamstasKitties.Shared.Tests.csproj
```

## Game Modes

- **Classic Mode** - Traditional match-3 gameplay
- **Countdown Mode** - Race against the clock
- **Gold Rush Mode** - Collect golden blocks
- **Chillout Mode** - Relaxed, no time pressure

## History

Originally developed for Windows Phone 7 using XNA, later ported to Android and iOS using Xamarin. This modern port brings the game to .NET 8 and MonoGame for continued support and cross-platform compatibility.

## License

Copyright © 2013-2026. All rights reserved.
