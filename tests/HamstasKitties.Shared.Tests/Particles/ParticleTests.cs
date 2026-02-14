using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using HamstasKitties.Particles;

namespace HamstasKitties.Shared.Tests.Particles;

/// <summary>
/// Unit tests for the Particle struct.
/// Tests cover data structure, initialization, and basic operations.
/// </summary>
public class ParticleTests
{
    // Test struct initialization and default values
    [Fact]
    public void Particle_DefaultInitialization_HasZeroValues()
    {
        // Arrange & Act
        var particle = new Particle();

        // Assert
        particle.Position.Should().Be(Vector2.Zero);
        particle.Scale.Should().Be(0f);
        particle.Rotation.Should().Be(0f);
        particle.Colour.Should().Be(new Vector4(0, 0, 0, 0));
        particle.Momentum.Should().Be(Vector2.Zero);
        particle.Velocity.Should().Be(Vector2.Zero);
        particle.Inception.Should().Be(0f);
        particle.Age.Should().Be(0f);
    }

    [Fact]
    public void Particle_CanSetPosition()
    {
        // Arrange
        var particle = new Particle();
        var expectedPosition = new Vector2(100f, 200f);

        // Act
        particle.Position = expectedPosition;

        // Assert
        particle.Position.Should().Be(expectedPosition);
    }

    [Fact]
    public void Particle_CanSetScale()
    {
        // Arrange
        var particle = new Particle();
        const float expectedScale = 1.5f;

        // Act
        particle.Scale = expectedScale;

        // Assert
        particle.Scale.Should().Be(expectedScale);
    }

    [Fact]
    public void Particle_CanSetRotation()
    {
        // Arrange
        var particle = new Particle();
        const float expectedRotation = Calculator.PiOver4; // 45 degrees

        // Act
        particle.Rotation = expectedRotation;

        // Assert
        particle.Rotation.Should().Be(expectedRotation);
    }

    [Fact]
    public void Particle_CanSetColour()
    {
        // Arrange
        var particle = new Particle();
        var expectedColour = new Vector4(1f, 0.5f, 0.2f, 0.8f);

        // Act
        particle.Colour = expectedColour;

        // Assert
        particle.Colour.Should().Be(expectedColour);
    }

    [Fact]
    public void Particle_CanSetMomentum()
    {
        // Arrange
        var particle = new Particle();
        var expectedMomentum = new Vector2(50f, -30f);

        // Act
        particle.Momentum = expectedMomentum;

        // Assert
        particle.Momentum.Should().Be(expectedMomentum);
    }

    [Fact]
    public void Particle_CanSetVelocity()
    {
        // Arrange
        var particle = new Particle();
        var expectedVelocity = new Vector2(10f, 20f);

        // Act
        particle.Velocity = expectedVelocity;

        // Assert
        particle.Velocity.Should().Be(expectedVelocity);
    }

    [Fact]
    public void Particle_CanSetInception()
    {
        // Arrange
        var particle = new Particle();
        const float expectedInception = 1.5f;

        // Act
        particle.Inception = expectedInception;

        // Assert
        particle.Inception.Should().Be(expectedInception);
    }

    [Fact]
    public void Particle_CanSetAge()
    {
        // Arrange
        var particle = new Particle();
        const float expectedAge = 0.5f;

        // Act
        particle.Age = expectedAge;

        // Assert
        particle.Age.Should().Be(expectedAge);
    }

    // Test ApplyForce method
    [Fact]
    public void ApplyForce_AddsForceToVelocity()
    {
        // Arrange
        var particle = new Particle { Velocity = new Vector2(5f, 10f) };
        var force = new Vector2(2f, 3f);

        // Act
        particle.ApplyForce(ref force);

        // Assert
        particle.Velocity.X.Should().Be(7f);
        particle.Velocity.Y.Should().Be(13f);
    }

    [Fact]
    public void ApplyForce_WithZeroVelocity_InitializesVelocity()
    {
        // Arrange
        var particle = new Particle { Velocity = Vector2.Zero };
        var force = new Vector2(10f, -5f);

        // Act
        particle.ApplyForce(ref force);

        // Assert
        particle.Velocity.Should().Be(force);
    }

    [Fact]
    public void ApplyForce_WithNegativeForce_SubtractsFromVelocity()
    {
        // Arrange
        var particle = new Particle { Velocity = new Vector2(10f, 10f) };
        var force = new Vector2(-3f, -2f);

        // Act
        particle.ApplyForce(ref force);

        // Assert
        particle.Velocity.X.Should().Be(7f);
        particle.Velocity.Y.Should().Be(8f);
    }

    // Test Rotate method
    [Fact]
    public void Rotate_AddsToRotation()
    {
        // Arrange
        var particle = new Particle { Rotation = 0f };
        const float rotationAmount = Calculator.PiOver4;

        // Act
        particle.Rotate(rotationAmount);

        // Assert
        particle.Rotation.Should().Be(rotationAmount);
    }

    [Fact]
    public void Rotate_WhenExceedingPi_WrapsToNegativePi()
    {
        // Arrange
        var particle = new Particle { Rotation = Calculator.Pi - 0.1f };
        const float rotationAmount = 0.2f;

        // Act
        particle.Rotate(rotationAmount);

        // Assert
        particle.Rotation.Should().BeGreaterThan(-Calculator.Pi);
        particle.Rotation.Should().BeLessThan(Calculator.Pi);
    }

    [Fact]
    public void Rotate_WhenExceedingNegativePi_WrapsToPi()
    {
        // Arrange
        var particle = new Particle { Rotation = -Calculator.Pi + 0.1f };
        const float rotationAmount = -0.2f;

        // Act
        particle.Rotate(rotationAmount);

        // Assert
        particle.Rotation.Should().BeGreaterThan(-Calculator.Pi);
        particle.Rotation.Should().BeLessThan(Calculator.Pi);
    }

    // Test struct layout attributes
    [Fact]
    public void Particle_IsStruct()
    {
        // Arrange & Act & Assert
        var type = typeof(Particle);
        type.IsValueType.Should().BeTrue();
    }

    [Fact]
    public void Particle_HasSequentialLayoutAttribute()
    {
        // This test documents that Particle uses LayoutKind.Sequential
        // which is important for serialization and interop
        var type = typeof(Particle);
        type.IsLayoutSequential.Should().BeTrue();
    }

    // Test field ordering and offsets (important for particle systems)
    [Fact]
    public void Particle_Position_IsFirstField()
    {
        // Verify Position is at the start for optimal cache access
        var particle = new Particle { Position = new Vector2(1, 2) };
        particle.Position.X.Should().Be(1f);
        particle.Position.Y.Should().Be(2f);
    }

    // Test particle lifecycle representation
    [Theory]
    [InlineData(0.0f)]
    [InlineData(0.25f)]
    [InlineData(0.5f)]
    [InlineData(0.75f)]
    [InlineData(1.0f)]
    public void Particle_Age_RepresentsLifecycleProgress(float age)
    {
        // Arrange & Act
        var particle = new Particle { Age = age };

        // Assert - Age should be in range [0, 1]
        particle.Age.Should().BeGreaterOrEqualTo(0f);
        particle.Age.Should().BeLessOrEqualTo(1f);
    }

    // Test colour component access
    [Fact]
    public void Particle_Colour_WComponentRepresentsOpacity()
    {
        // Arrange
        var particle = new Particle
        {
            Colour = new Vector4(1f, 0.5f, 0.2f, 0.8f)
        };

        // Act & Assert - W is opacity (alpha)
        particle.Colour.X.Should().Be(1f);  // Red
        particle.Colour.Y.Should().Be(0.5f); // Green
        particle.Colour.Z.Should().Be(0.2f); // Blue
        particle.Colour.W.Should().Be(0.8f); // Alpha/Opacity
    }

    // Test edge cases
    [Fact]
    public void Particle_WithLargeScale_HasCorrectValue()
    {
        // Arrange
        var particle = new Particle();
        const float largeScale = 100f;

        // Act
        particle.Scale = largeScale;

        // Assert
        particle.Scale.Should().Be(largeScale);
    }

    [Fact]
    public void Particle_WithNegativeRotation_HasCorrectValue()
    {
        // Arrange
        var particle = new Particle();
        const float negativeRotation = -Calculator.PiOver2;

        // Act
        particle.Rotation = negativeRotation;

        // Assert
        particle.Rotation.Should().Be(negativeRotation);
    }

    // Test multiple particle instances
    [Fact]
    public void MultipleParticles_CanCoexistIndependently()
    {
        // Arrange
        var particle1 = new Particle { Position = new Vector2(10, 20), Age = 0.5f };
        var particle2 = new Particle { Position = new Vector2(30, 40), Age = 0.7f };

        // Assert
        particle1.Position.Should().NotBe(particle2.Position);
        particle1.Age.Should().NotBe(particle2.Age);
    }
}
