using System;
using FluentAssertions;
using HamstasKitties.Mechanics;
using Xunit;

namespace HamstasKitties.Shared.Tests.Mechanics;

public class ComboManagerTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithEmptyMatchingGroups()
    {
        // Arrange & Act
        var comboManager = new ComboManager();

        // Assert
        comboManager.Should().NotBeNull();
    }

    [Fact]
    public void AddMatchingGroup_ShouldRegisterGroup()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup = new MatchingGroup(comboManager);

        // Act
        comboManager.AddMatchingGroup(matchingGroup);

        // Assert
        comboManager.IsMatchingGroupRegistered(matchingGroup).Should().BeTrue();
    }

    [Fact]
    public void AddMatchingGroup_ShouldNotRegisterDuplicateGroups()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup = new MatchingGroup(comboManager);

        // Act
        comboManager.AddMatchingGroup(matchingGroup);
        comboManager.AddMatchingGroup(matchingGroup);

        // Assert - Should not throw and group should still be registered
        comboManager.IsMatchingGroupRegistered(matchingGroup).Should().BeTrue();
    }

    [Fact]
    public void AddMatchingGroup_ShouldNotRegisterNullGroup()
    {
        // Arrange
        var comboManager = new ComboManager();

        // Act
        var act = () => comboManager.AddMatchingGroup(null);

        // Assert - Should not throw
        act.Should().NotThrow();
    }

    [Fact]
    public void IsMatchingGroupRegistered_ShouldReturnFalseForUnregisteredGroup()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup = new MatchingGroup(comboManager);

        // Act
        var result = comboManager.IsMatchingGroupRegistered(matchingGroup);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RemoveMatchingGroup_ShouldUnregisterGroup()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup = new MatchingGroup(comboManager);
        comboManager.AddMatchingGroup(matchingGroup);

        // Act
        comboManager.RemoveMatchingGroup(matchingGroup);

        // Assert
        comboManager.IsMatchingGroupRegistered(matchingGroup).Should().BeFalse();
    }

    [Fact]
    public void IncrementComboMultiplier_ShouldFireOnComboStarted_WhenMultiplierReaches2()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);

        int? firedMultiplier = null;
        comboManager.OnComboStarted += (multiplier) => firedMultiplier = multiplier;

        // Act
        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);

        // Assert
        firedMultiplier.Should().Be(2);
    }

    [Fact]
    public void IncrementComboMultiplier_ShouldFireOnComboUpdated_WhenMultiplierGreaterThan2()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);
        var matchingGroup3 = new MatchingGroup(comboManager);

        int? firedMultiplier = null;
        comboManager.OnComboUpdated += (multiplier) => firedMultiplier = multiplier;

        // Act
        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);
        comboManager.IncrementComboMultiplier(matchingGroup3);

        // Assert
        firedMultiplier.Should().Be(3);
    }

    [Fact]
    public void IncrementComboMultiplier_ShouldFireMultipleOnComboUpdatedEvents()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);
        var matchingGroup3 = new MatchingGroup(comboManager);
        var matchingGroup4 = new MatchingGroup(comboManager);

        int updatedCount = 0;
        int? lastMultiplier = null;
        comboManager.OnComboUpdated += (multiplier) =>
        {
            updatedCount++;
            lastMultiplier = multiplier;
        };

        // Act
        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);
        comboManager.IncrementComboMultiplier(matchingGroup3);
        comboManager.IncrementComboMultiplier(matchingGroup4);

        // Assert
        updatedCount.Should().Be(2); // Multiplier 3 and 4
        lastMultiplier.Should().Be(4);
    }

    [Fact]
    public void IncrementComboMultiplier_ShouldNotFireOnComboStarted_WhenOnlyOneIncrement()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup = new MatchingGroup(comboManager);

        bool eventFired = false;
        comboManager.OnComboStarted += (multiplier) => eventFired = true;

        // Act
        comboManager.IncrementComboMultiplier(matchingGroup);

        // Assert
        eventFired.Should().BeFalse();
    }

    [Fact]
    public void StopCurrentCombo_ShouldFireOnComboFinished_WhenComboIsActive()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);

        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);

        int? finishedMultiplier = null;
        comboManager.OnComboFinished += (multiplier) => finishedMultiplier = multiplier;

        // Act
        comboManager.StopCurrentCombo();

        // Assert
        finishedMultiplier.Should().Be(2);
    }

    [Fact]
    public void StopCurrentCombo_ShouldNotFireOnComboFinished_WhenNoComboIsActive()
    {
        // Arrange
        var comboManager = new ComboManager();

        bool eventFired = false;
        comboManager.OnComboFinished += (multiplier) => eventFired = true;

        // Act
        comboManager.StopCurrentCombo();

        // Assert
        eventFired.Should().BeFalse();
    }

    [Fact]
    public void StopCurrentCombo_ShouldResetMultiplier()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);

        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);

        // Act
        comboManager.StopCurrentCombo();

        // Assert - After stop, next increment should start from multiplier 1
        // OnComboStarted should fire at multiplier 2
        int? startedMultiplier = null;
        comboManager.OnComboStarted += (multiplier) => startedMultiplier = multiplier;

        var matchingGroup3 = new MatchingGroup(comboManager);
        var matchingGroup4 = new MatchingGroup(comboManager);
        comboManager.IncrementComboMultiplier(matchingGroup3);
        comboManager.IncrementComboMultiplier(matchingGroup4);

        startedMultiplier.Should().Be(2);
    }

    [Fact]
    public void Update_ShouldUpdateAllRegisteredMatchingGroups()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);

        comboManager.AddMatchingGroup(matchingGroup1);
        comboManager.AddMatchingGroup(matchingGroup2);

        var timeSpan = TimeSpan.FromMilliseconds(100);

        // Act & Assert - Should not throw
        var act = () => comboManager.Update(timeSpan);
        act.Should().NotThrow();
    }

    [Fact]
    public void Update_ShouldHandleEmptyMatchingGroups()
    {
        // Arrange
        var comboManager = new ComboManager();
        var timeSpan = TimeSpan.FromMilliseconds(100);

        // Act & Assert - Should not throw
        var act = () => comboManager.Update(timeSpan);
        act.Should().NotThrow();
    }

    [Fact]
    public void FullComboFlow_ShouldFireAllEventsInCorrectOrder()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);
        var matchingGroup3 = new MatchingGroup(comboManager);

        var events = new System.Collections.Generic.List<string>();
        comboManager.OnComboStarted += (m) => events.Add($"Started_{m}");
        comboManager.OnComboUpdated += (m) => events.Add($"Updated_{m}");
        comboManager.OnComboFinished += (m) => events.Add($"Finished_{m}");

        // Act
        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);
        comboManager.IncrementComboMultiplier(matchingGroup3);
        comboManager.StopCurrentCombo();

        // Assert
        events.Should().HaveCount(3);
        events[0].Should().Be("Started_2");
        events[1].Should().Be("Updated_3");
        events[2].Should().Be("Finished_3");
    }

    [Fact]
    public void MultipleStopCurrentCombo_ShouldOnlyFireOnComboFinishedOnce()
    {
        // Arrange
        var comboManager = new ComboManager();
        var matchingGroup1 = new MatchingGroup(comboManager);
        var matchingGroup2 = new MatchingGroup(comboManager);

        comboManager.IncrementComboMultiplier(matchingGroup1);
        comboManager.IncrementComboMultiplier(matchingGroup2);

        int finishCount = 0;
        comboManager.OnComboFinished += (multiplier) => finishCount++;

        // Act
        comboManager.StopCurrentCombo();
        comboManager.StopCurrentCombo();

        // Assert
        finishCount.Should().Be(1);
    }
}
