using Xunit;
using FluentAssertions;
using HamstasKitties.Constants;

namespace HamstasKitties.Shared.Tests
{
    public class GlobalConstantsTests
    {
        [Fact]
        public void Version_ShouldBeDefined()
        {
            GlobalConstants.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GameTitle_ShouldBeHamstasNKitties()
        {
            GlobalConstants.GameTitle.Should().Be("Hamstas'n'Kitties");
        }

        [Fact]
        public void CompanyName_ShouldBeDagariStudios()
        {
            GlobalConstants.CompanyName.Should().Be("Dagari Studios");
        }

        [Fact]
        public void DefaultSceneWidth_ShouldBe480()
        {
            GlobalConstants.DefaultSceneWidth.Should().Be(480);
        }

        [Fact]
        public void DefaultSceneHeight_ShouldBe800()
        {
            GlobalConstants.DefaultSceneHeight.Should().Be(800);
        }

        [Fact]
        public void NumberOfBlockGridColumns_ShouldBe7()
        {
            GlobalConstants.NumberOfBlockGridColumns.Should().Be(7);
        }

        [Fact]
        public void NumberOfBlockGridRows_ShouldBe8()
        {
            GlobalConstants.NumberOfBlockGridRows.Should().Be(8);
        }

        [Fact]
        public void MinBlocksToMatch_ShouldBe3()
        {
            GlobalConstants.MinBlocksToMatch.Should().Be(3);
        }

        [Fact]
        public void BlockSize_ShouldBe64()
        {
            GlobalConstants.BlockSize.Should().Be(64);
        }
    }

    public class ScoreConstantsTests
    {
        [Fact]
        public void BoardClearedPoints_ShouldBe500()
        {
            ScoreConstants.BoardClearedPoints.Should().Be(500);
        }

        [Fact]
        public void RemovedRegularBlockPoints_ShouldBe10()
        {
            ScoreConstants.RemovedRegularBlockPoints.Should().Be(10);
        }

        [Fact]
        public void UnmovableBlockPoints_ShouldBe50()
        {
            ScoreConstants.UnmovableBlockPoints.Should().Be(50);
        }

        [Fact]
        public void RemovedPowerUpBlockPoints_ShouldBe20()
        {
            ScoreConstants.RemovedPowerUpBlockPoints.Should().Be(20);
        }

        [Fact]
        public void GoldenBlockPoints_ShouldBe50()
        {
            ScoreConstants.GoldenBlockPoints.Should().Be(50);
        }

        [Fact]
        public void ComboMultiPoints_ShouldBe100()
        {
            ScoreConstants.ComboMultiPoints.Should().Be(100);
        }
    }

    public class UILayoutConstantsTests
    {
        [Fact]
        public void LayersVerticalGap_ShouldBe0()
        {
            UILayoutConstants.LayersVerticalGap.Should().Be(0);
        }

        [Fact]
        public void MenuElementsLeftMargin_ShouldBe10()
        {
            UILayoutConstants.MenuElementsLeftMargin.Should().Be(10);
        }

        [Fact]
        public void MenuTextRelativeMargin_ShouldBe45()
        {
            UILayoutConstants.MenuTextRelativeMargin.Should().Be(45);
        }

        [Fact]
        public void MenuTopMargin_ShouldBe15()
        {
            UILayoutConstants.MenuTopMargin.Should().Be(15);
        }

        [Fact]
        public void AdsHeight_ShouldBe80()
        {
            UILayoutConstants.AdsHeight.Should().Be(80);
        }
    }

    public class PersistableSettingsConstantsTests
    {
        [Fact]
        public void GameStateKey_ShouldBeDefined()
        {
            PersistableSettingsConstants.GameStateKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GameStateKey_ShouldStartWithDSHK()
        {
            PersistableSettingsConstants.GameStateKey.Should().StartWith("DSHK");
        }

        [Fact]
        public void FirstLaunchKey_ShouldBeDefined()
        {
            PersistableSettingsConstants.FirstLaunchKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TutorialAlreadyOpenedKey_ShouldBeDefined()
        {
            PersistableSettingsConstants.TutorialAlreadyOpenedKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TotalRemovedBlocksKey_ShouldBeDefined()
        {
            PersistableSettingsConstants.TotalRemovedBlocksKey.Should().NotBeNullOrEmpty();
        }
    }

    public class AnalyticsConstantsTests
    {
        [Fact]
        public void FlurryAnalyticsAPIKey_ShouldBeDefined()
        {
            AnalyticsConstants.FlurryAnalyticsAPIKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Events_ShouldContainStartedPlaying()
        {
            AnalyticsConstants.Events.StartedPlaying.Should().Be(AnalyticsConstants.Events.StartedPlaying);
        }

        [Fact]
        public void Events_ShouldContainFinishedPlaying()
        {
            AnalyticsConstants.Events.FinishedPlaying.Should().Be(AnalyticsConstants.Events.FinishedPlaying);
        }

        [Fact]
        public void Events_ShouldContainAchievementUnlocked()
        {
            AnalyticsConstants.Events.AchievementUnlocked.Should().Be(AnalyticsConstants.Events.AchievementUnlocked);
        }

        [Fact]
        public void Events_ShouldContainScoreSubmitted()
        {
            AnalyticsConstants.Events.ScoreSubmitted.Should().Be(AnalyticsConstants.Events.ScoreSubmitted);
        }

        [Fact]
        public void EventParameters_ShouldContainSessionUUID()
        {
            AnalyticsConstants.EventParameters.SessionUUID.Should().Be(AnalyticsConstants.EventParameters.SessionUUID);
        }

        [Fact]
        public void EventParameters_ShouldContainScore()
        {
            AnalyticsConstants.EventParameters.Score.Should().Be(AnalyticsConstants.EventParameters.Score);
        }
    }

    public class ScoreloopConstantsTests
    {
        [Fact]
        public void GameId_ShouldBeDefined()
        {
            ScoreloopConstants.GameId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GameSecret_ShouldBeDefined()
        {
            ScoreloopConstants.GameSecret.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GameCurrency_ShouldBeBAS()
        {
            ScoreloopConstants.GameCurrency.Should().Be("BAS");
        }
    }
}
