using System.ComponentModel;
using FluentAssertions;
using HamstasKitties.Core.Mocks;
using Xunit;

namespace HamstasKitties.Shared.Tests.Services;

public class MockNetworkServiceTests
{
    [Fact]
    public void Default_IsNetworkAvailable_IsTrue()
    {
        // Arrange
        var service = new MockNetworkService();

        // Assert
        service.IsNetworkAvailable.Should().BeTrue();
    }

    [Fact]
    public void IsNetworkAvailable_CanBeSetToFalse()
    {
        // Arrange
        var service = new MockNetworkService();

        // Act
        service.IsNetworkAvailable = false;

        // Assert
        service.IsNetworkAvailable.Should().BeFalse();
    }

    [Fact]
    public void Initialize_DoesNotThrow()
    {
        // Arrange
        var service = new MockNetworkService();

        // Act & Assert
        service.Invoking(s => s.Initialize()).Should().NotThrow();
    }

    [Fact]
    public void CheckConnection_IncrementsCallCount()
    {
        // Arrange
        var service = new MockNetworkService();

        // Act
        service.CheckConnection();

        // Assert
        service.CheckConnectionCallCount.Should().Be(1);
    }

    [Fact]
    public void CheckConnection_CalledMultipleTimes_IncrementsCallCountAccordingly()
    {
        // Arrange
        var service = new MockNetworkService();

        // Act
        service.CheckConnection();
        service.CheckConnection();
        service.CheckConnection();

        // Assert
        service.CheckConnectionCallCount.Should().Be(3);
    }

    [Fact]
    public void SimulateOnline_RaisesOnNetworkOnlineEvent()
    {
        // Arrange
        var service = new MockNetworkService();
        object? sender = null;
        EventArgs? args = null;

        service.OnNetworkOnline += (s, e) =>
        {
            sender = s;
            args = e;
        };

        // Act
        service.SimulateOnline();

        // Assert
        sender.Should().Be(service);
        args.Should().Be(EventArgs.Empty);
    }

    [Fact]
    public void SimulateOffline_RaisesOnNetworkOfflineEvent()
    {
        // Arrange
        var service = new MockNetworkService();
        object? sender = null;
        EventArgs? args = null;

        service.OnNetworkOffline += (s, e) =>
        {
            sender = s;
            args = e;
        };

        // Act
        service.SimulateOffline();

        // Assert
        sender.Should().Be(service);
        args.Should().Be(EventArgs.Empty);
    }

    [Fact]
    public void SimulateOnline_MultipleSubscribers_AllSubscribersReceiveEvent()
    {
        // Arrange
        var service = new MockNetworkService();
        var callCount1 = 0;
        var callCount2 = 0;

        service.OnNetworkOnline += (s, e) => callCount1++;
        service.OnNetworkOnline += (s, e) => callCount2++;

        // Act
        service.SimulateOnline();

        // Assert
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
    }

    [Fact]
    public void SimulateOffline_MultipleSubscribers_AllSubscribersReceiveEvent()
    {
        // Arrange
        var service = new MockNetworkService();
        var callCount1 = 0;
        var callCount2 = 0;

        service.OnNetworkOffline += (s, e) => callCount1++;
        service.OnNetworkOffline += (s, e) => callCount2++;

        // Act
        service.SimulateOffline();

        // Assert
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
    }

    [Fact]
    public void OnNetworkOnlineEvent_WhenNoSubscribers_DoesNotThrow()
    {
        // Arrange
        var service = new MockNetworkService();

        // Act & Assert
        service.Invoking(s => s.SimulateOnline()).Should().NotThrow();
    }

    [Fact]
    public void OnNetworkOfflineEvent_WhenNoSubscribers_DoesNotThrow()
    {
        // Arrange
        var service = new MockNetworkService();

        // Act & Assert
        service.Invoking(s => s.SimulateOffline()).Should().NotThrow();
    }

    [Fact]
    public void CheckConnectionCallCount_InitiallyZero()
    {
        // Arrange
        var service = new MockNetworkService();

        // Assert
        service.CheckConnectionCallCount.Should().Be(0);
    }
}
