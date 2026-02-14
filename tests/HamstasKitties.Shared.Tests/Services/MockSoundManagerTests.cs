using FluentAssertions;
using HamstasKitties.Core.Mocks;
using Xunit;

namespace HamstasKitties.Shared.Tests.Services;

public class MockSoundManagerTests
{
    [Fact]
    public void Initialize_ReturnsTrue()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        var result = manager.Initialize();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Finalize_ReturnsTrue()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        var result = manager.Finalize();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Assert
        manager.IsMusicEnabled.Should().BeTrue();
        manager.IsSoundFXEnabled.Should().BeTrue();
        manager.MasterVolume.Should().Be(1.0f);
    }

    [Fact]
    public void PlaySound_IncrementsCallCount()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.PlaySound("test");

        // Assert
        manager.PlaySoundCallCount.Should().Be(1);
    }

    [Fact]
    public void PlaySound_CalledMultipleTimes_IncrementsCallCountAccordingly()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.PlaySound("test1");
        manager.PlaySound("test2");
        manager.PlaySound("test3");

        // Assert
        manager.PlaySoundCallCount.Should().Be(3);
    }

    [Fact]
    public void PlaySong_IncrementsCallCount()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.PlaySong("song", true);

        // Assert
        manager.PlaySongCallCount.Should().Be(1);
    }

    [Fact]
    public void PlaySong_WithLoopFalse_IncrementsCallCount()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.PlaySong("song", false);

        // Assert
        manager.PlaySongCallCount.Should().Be(1);
    }

    [Fact]
    public void IsMusicEnabled_CanBeSetToFalse()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.IsMusicEnabled = false;

        // Assert
        manager.IsMusicEnabled.Should().BeFalse();
    }

    [Fact]
    public void IsSoundFXEnabled_CanBeSetToFalse()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.IsSoundFXEnabled = false;

        // Assert
        manager.IsSoundFXEnabled.Should().BeFalse();
    }

    [Fact]
    public void MasterVolume_CanBeSet()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act
        manager.MasterVolume = 0.5f;

        // Assert
        manager.MasterVolume.Should().Be(0.5f);
    }

    [Fact]
    public void StopCurrentSong_DoesNotThrow()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act & Assert
        manager.Invoking(m => m.StopCurrentSong()).Should().NotThrow();
    }

    [Fact]
    public void PauseCurrentSong_DoesNotThrow()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act & Assert
        manager.Invoking(m => m.PauseCurrentSong()).Should().NotThrow();
    }

    [Fact]
    public void ResumeCurrentSong_DoesNotThrow()
    {
        // Arrange
        var manager = new MockSoundManager();

        // Act & Assert
        manager.Invoking(m => m.ResumeCurrentSong()).Should().NotThrow();
    }
}
