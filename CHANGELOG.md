# Hamstas'n'Kitties Port - Changelog

All changes for the MonoGame modernization port from legacy Xamarin to modern .NET 8 / MonoGame.

---

## [Unreleased]

### Phase 4: Platform Projects
*Started: 2026-02-14*
*Completed: 2026-02-14*

#### Added
- **Android Project** (`src/HamstasKitties.Android/`):
  - `.NET 8 Android` project targeting API 24+
  - `MainActivity.cs` - Entry point with lifecycle handling
  - `Game1.cs` - MonoGame Android game class
  - `AndroidManifest.xml` - App configuration
  - **Platform Services**:
    - `AndroidVibratorService.cs` - Haptic feedback implementation
    - `AndroidNetworkService.cs` - Connectivity monitoring
- **iOS Project** (`src/HamstasKitties.iOS/`):
  - `.NET 8 iOS` project targeting iOS 12.0+
  - `Program.cs` / `AppDelegate.cs` - Entry point with lifecycle
  - `Game1.cs` - MonoGame iOS game class
  - `Info.plist` - App configuration
  - **Platform Services**:
    - `iOSVibratorService.cs` - Haptic feedback implementation
    - `iOSNetworkService.cs` - NWPathMonitor connectivity

#### Changed
- Solution now includes 4 projects: Shared, Desktop, Android, iOS

### Phase 3: UI Framework Migration
*Started: 2026-02-14*
*Completed: 2026-02-14*

#### Added
- **UI Components** (`src/HamstasKitties.Shared/UI/`):
  - `Button.cs`, `PushButton.cs`, `SelectableButton.cs`, `SimpleTextureButton.cs`
  - `Text.cs`, `BitmapText.cs` - Text rendering
  - `ListView.cs`, `ListViewItem.cs` - Scrollable lists
  - `PageableLayer.cs`, `LayerGroup.cs`, `LeaderboardItem.cs`
- **Game Scenes** (`src/HamstasKitties.Shared/Scenes/`):
  - Core: `SplashScreen.cs`, `IntroScene.cs`, `AchievementPopup.cs`
  - GameModes: `Level.cs`, `ClassicMode.cs`, `CountdownMode.cs`, `GoldRushMode.cs`, `ChilloutMode.cs`
  - Menus: `MainMenu.cs`, `NewGameMenu.cs`, `OptionsMenu.cs`, `AboutMenu.cs`, `AchievementsMenu.cs`, `LeadersboardMenu.cs`, `LevelPauseMenu.cs`, `LevelGameOverMenu.cs`, `TutorialMenu.cs`
- **Game Layers** (`src/HamstasKitties.Shared/Layers/`):
  - Core: `HUDLayer.cs`, `TimeRemainingLayer.cs`, `LevelBackgroundPanelLayer.cs`, `LevelBlocksPanelLayer.cs`
  - Menus: `MainMenuTitleLayer.cs`, `MainMenuGameModesButtonsLayer.cs`, `MainMenuOptionsButtonsLayer.cs`, `NewGameMenuButtonsLayer.cs`
  - Pause/GameOver: `LevelPauseMenuLayer.cs`, `LevelGameOverMenuLayer.cs`
  - Options/Achievements/Leaderboard layers

#### Changed
- All namespaces updated from `GameLibrary.*` to `HamstasKitties.*`
- 99 total source files in shared project

### Phase 2: Shared Code Migration
*Started: 2026-02-14*
*Completed: 2026-02-14*

#### Added
- **Core Framework** (`src/HamstasKitties.Shared/Core/`):
  - `Director.cs` - Abstract game director with scene management
  - `ScreenResolutionManager.cs` - Resolution scaling
  - `DeviceInfo.cs` - Device information stubs
- **UI Framework** (`src/HamstasKitties.Shared/UI/`):
  - `LayerObject.cs` - Base renderable/touchable class
  - `Layer.cs`, `Scene.cs` - Scene graph components
  - `Texture.cs`, `ITouchable.cs`, `IUpdateable.cs`
- **Animation** (`src/HamstasKitties.Shared/Animation/`):
  - `Timer.cs` - Animation timer with events
  - Tween functions (Back, Bounce, Circular, etc.)
- **Game Mechanics** (`src/HamstasKitties.Shared/Mechanics/`):
  - `Block.cs` (1326 lines) - Core block logic, WINDOWS_PHONE code removed
  - `LevelBoardController.cs` (718 lines) - Match-3 board management
  - `MatchingGroup.cs`, `ComboManager.cs` - Match grouping and combo system
  - `BlocksMiniPhysicsEngine.cs` - Collision detection
- **State Management** (`src/HamstasKitties.Shared/Management/`):
  - `GameStateManager.cs`, `GameState.cs`, `GameModeState.cs`
  - `BlockState.cs`, `BoardState.cs`, `BestScore.cs`
- **Utilities** (`src/HamstasKitties.Shared/Utils/`):
  - `Rand.cs` - Static random number generator
  - `PhysicsTools.cs` - Line intersection, distance calculations

#### Removed
- All particle effect code (ProjectMercury) from Block.cs
- All `#if WINDOWS_PHONE` conditionals from migrated files

#### Changed
- All namespaces updated from `GameLibrary.*` to `HamstasKitties.*`
- 50 total source files migrated to shared project

### Phase 1: Core Abstractions
*Started: 2026-02-14*
*Completed: 2026-02-14*

#### Added
- `src/HamstasKitties.Shared/Core/Interfaces/` - Platform-agnostic interfaces:
  - `IManager.cs` - Base manager interface
  - `ISoundManager.cs` - Audio playback abstraction
  - `ISettingsManager.cs` - Settings persistence abstraction
  - `INetworkService.cs` - Network connectivity abstraction
  - `IDeviceInfoService.cs` - Device information abstraction
  - `IVibratorService.cs` - Haptic feedback abstraction (mobile)
  - `IAccelerometerService.cs` - Motion sensor abstraction
  - `IAnalyticsService.cs` - Analytics abstraction
  - `IAchievementService.cs` - Achievements abstraction
  - `ILeaderboardService.cs` - Leaderboard abstraction
  - `ITouchPanelService.cs` - Touch input abstraction
- `src/HamstasKitties.Shared/Core/Mocks/` - Test mock implementations:
  - `MockSoundManager.cs`, `MockSettingsManager.cs`, `MockVibratorService.cs`
  - `MockNetworkService.cs`, `MockAnalyticsService.cs`

#### Removed
- Windows Phone-specific code:
  - `AccelerometerManager.cs`, `VibratorManager.cs`, `ApplicationLifeCycleManager.cs`
  - `FlurryAnalyticsService.cs`, `ScoreloopService.cs`, `TasksUtils.cs`
- All `#if WINDOWS_PHONE` conditionals from core library

#### Changed
- Cleaned `Director.cs`, `NetworkManager.cs`, `DeviceInfo.cs`, `DeviceStatus.cs`
- Simplified `DataContractSerializer.cs` for cross-platform use

### Phase 0: Project Setup
*Started: 2026-02-14*
*Completed: 2026-02-14*

#### Added
- Initial CHANGELOG.md for tracking all port changes
- New SDK-style .NET 8 solution structure (`HamstasKitties.sln`)
- `src/HamstasKitties.Shared/` - Shared game library project
- `src/HamstasKitties.Desktop/` - Desktop MonoGame project with Content pipeline
- `tests/HamstasKitties.Shared.Tests/` - xUnit test project with FluentAssertions
- `.github/workflows/build.yml` - CI/CD pipeline (build + test + lint)
- `Directory.Build.props` - Common build settings (warnings as errors, code style)
- `.editorconfig` - Editor configuration for consistent formatting

#### Changed
- Renamed legacy `HamstasKitties.sln` to `HamstasKitties.Legacy.sln`

---

## Change Log Format

Each change entry follows this format:

```
#### YYYY-MM-DD - Brief Description
- **Phase:** X
- **Files:** path/to/file1.cs, path/to/file2.cs
- **Description:** Detailed description of the change
- **Breaking Changes:** None | Description of breaking changes
```

---

## Phase Progress

| Phase | Status | Start Date | End Date |
|-------|--------|------------|----------|
| 0: Project Setup | Complete | 2026-02-14 | 2026-02-14 |
| 1: Core Abstractions | Complete | 2026-02-14 | 2026-02-14 |
| 2: Shared Code Migration | Complete | 2026-02-14 | 2026-02-14 |
| 3: UI Framework Migration | Complete | 2026-02-14 | 2026-02-14 |
| 4: Platform Projects | Complete | 2026-02-14 | 2026-02-14 |
| 5: Firebase Integration | Not Started | - | - |
| 6: Assets Migration | Not Started | - | - |
| 7: Testing & QA | Not Started | - | - |
| 8: Polish & Deploy | Not Started | - | - |

---

## Technical Debt Resolved

| Issue | Location | Resolution | Date |
|-------|----------|------------|------|
| Windows Phone platform lock-in | Core managers | Removed with interfaces | 2026-02-14 |
| Dead code (Scoreloop, Flurry) | Social/, Analytics/ | Deleted | 2026-02-14 |

---

## Deprecations Removed

| Service/Library | Replaced By | Date |
|-----------------|-------------|------|
| Scoreloop | ILeaderboardService (interface only) | 2026-02-14 |
| Flurry Analytics | IAnalyticsService (interface only) | 2026-02-14 |
| Windows Phone conditionals | Removed | 2026-02-14 |

---

## Notes

- All dates in YYYY-MM-DD format
- Breaking changes require major version bump
- Each phase completion requires code review sign-off
