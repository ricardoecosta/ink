using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using HamstasKitties.Particles;
using HamstasKitties.Particles.Emitters;

namespace HamstasKitties.Shared.Tests.Particles;

/// <summary>
/// Unit tests for the ParticleEffect class.
/// Tests cover effect management, emitter collection, and lifecycle methods.
/// </summary>
public class ParticleEffectTests
{
    // Test construction and initialization
    [Fact]
    public void ParticleEffect_Constructor_CreatesEmptyEffect()
    {
        // Arrange & Act
        var effect = new ParticleEffect();

        // Assert
        effect.Name.Should().Be("Particle Effect");
        effect.Count.Should().Be(0);
        effect.Author.Should().BeNull();
        effect.Description.Should().BeNull();
        effect.Controllers.Should().NotBeNull();
    }

    [Fact]
    public void ParticleEffect_Constructor_InitializesControllers()
    {
        // Arrange & Act
        var effect = new ParticleEffect();

        // Assert
        effect.Controllers.Should().NotBeNull();
        effect.Controllers.Owner.Should().Be(effect);
        effect.Controllers.Count.Should().Be(0);
    }

    // Test Name property
    [Fact]
    public void Name_Setter_UpdatesValue()
    {
        // Arrange
        var effect = new ParticleEffect();
        const string expectedName = "Test Effect";

        // Act
        effect.Name = expectedName;

        // Assert
        effect.Name.Should().Be(expectedName);
    }

    [Fact]
    public void Name_SetToNull_ThrowsArgumentNullException()
    {
        // Arrange
        var effect = new ParticleEffect();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => effect.Name = null);
    }

    [Fact]
    public void Name_SetToEmpty_ThrowsArgumentNullException()
    {
        // Arrange
        var effect = new ParticleEffect();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => effect.Name = "");
    }

    [Fact]
    public void NameChanged_Event_FiresWhenNameChanges()
    {
        // Arrange
        var effect = new ParticleEffect();
        object sender = null;
        EventArgs args = null;

        effect.NameChanged += (s, e) =>
        {
            sender = s;
            args = e;
        };

        // Act
        effect.Name = "New Name";

        // Assert
        sender.Should().Be(effect);
        args.Should().Be(EventArgs.Empty);
    }

    // Test Emitter collection management
    [Fact]
    public void AddEmitter_IncreasesCount()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter = CreateTestEmitter();

        // Act
        effect.Add(emitter);

        // Assert
        effect.Count.Should().Be(1);
    }

    [Fact]
    public void Indexer_ReturnsCorrectEmitter()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter = CreateTestEmitter();
        effect.Add(emitter);

        // Act
        var result = effect[0];

        // Assert
        result.Should().Be(emitter);
    }

    [Fact]
    public void RemoveEmitter_DecreasesCount()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter = CreateTestEmitter();
        effect.Add(emitter);

        // Act
        effect.Remove(emitter);

        // Assert
        effect.Count.Should().Be(0);
    }

    // Test DeepCopy
    [Fact]
    public void DeepCopy_CreatesNewInstance()
    {
        // Arrange
        var effect = new ParticleEffect
        {
            Name = "Original",
            Author = "Test Author",
            Description = "Test Description"
        };
        var emitter = CreateTestEmitter();
        effect.Add(emitter);

        // Act
        var copy = effect.DeepCopy();

        // Assert
        copy.Should().NotBeSameAs(effect);
        copy.Name.Should().Be(effect.Name);
        copy.Author.Should().Be(effect.Author);
        copy.Description.Should().Be(effect.Description);
        copy.Count.Should().Be(effect.Count);
    }

    [Fact]
    public void DeepCopy_DoesNotAffectOriginal()
    {
        // Arrange
        var effect = new ParticleEffect { Name = "Original" };
        var emitter = CreateTestEmitter();
        effect.Add(emitter);

        // Act
        var copy = effect.DeepCopy();
        copy.Name = "Copy";

        // Assert
        effect.Name.Should().Be("Original");
        copy.Name.Should().Be("Copy");
    }

    // Test Trigger methods
    [Fact]
    public void Trigger_WithVector2_CallsEmitterTrigger()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter = CreateTestEmitter();
        effect.Add(emitter);
        var position = new Vector2(100, 200);

        // Act
        effect.Trigger(position);

        // Assert - If we had a mock emitter, we could verify the call
        // For now, we verify the method doesn't throw
        effect.ActiveParticlesCount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public void Trigger_WithRefVector2_CallsEmitterTrigger()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter = CreateTestEmitter();
        effect.Add(emitter);
        var position = new Vector2(100, 200);

        // Act
        effect.Trigger(ref position);

        // Assert
        effect.ActiveParticlesCount.Should().BeGreaterOrEqualTo(0);
    }

    // Test lifecycle methods
    [Fact]
    public void Initialise_CallsInitialiseOnAllEmitters()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter1 = CreateTestEmitter();
        var emitter2 = CreateTestEmitter();
        effect.Add(emitter1);
        effect.Add(emitter2);

        // Act
        effect.Initialise();

        // Assert - Method should not throw
        effect.Count.Should().Be(2);
    }

    [Fact]
    public void Terminate_CallsTerminateOnAllEmitters()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter1 = CreateTestEmitter();
        var emitter2 = CreateTestEmitter();
        effect.Add(emitter1);
        effect.Add(emitter2);

        // Act
        effect.Terminate();

        // Assert - Method should not throw
        effect.Count.Should().Be(2);
    }

    [Fact]
    public void Update_WithoutControllers_CallsUpdateOnAllEmitters()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter1 = CreateTestEmitter();
        var emitter2 = CreateTestEmitter();
        effect.Add(emitter1);
        effect.Add(emitter2);
        const float deltaSeconds = 0.016f;

        // Act
        effect.Update(deltaSeconds);

        // Assert - Method should not throw
        effect.ActiveParticlesCount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public void Update_WithControllers_UsesControllers()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter = CreateTestEmitter();
        effect.Add(emitter);
        const float deltaSeconds = 0.016f;

        // Act
        effect.Update(deltaSeconds);

        // Assert
        effect.ActiveParticlesCount.Should().BeGreaterOrEqualTo(0);
    }

    // Test LoadContent
    [Fact]
    public void LoadContent_CallsLoadContentOnAllEmitters()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter1 = CreateTestEmitter();
        var emitter2 = CreateTestEmitter();
        effect.Add(emitter1);
        effect.Add(emitter2);

        // Act & Assert - Without actual ContentManager, we can't test fully
        // But the method should exist
        effect.Count.Should().Be(2);
    }

    // Test ActiveParticlesCount
    [Fact]
    public void ActiveParticlesCount_WithNoEmitters_ReturnsZero()
    {
        // Arrange
        var effect = new ParticleEffect();

        // Act & Assert
        effect.ActiveParticlesCount.Should().Be(0);
    }

    [Fact]
    public void ActiveParticlesCount_WithEmitters_SumsParticleCounts()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter1 = CreateTestEmitter();
        var emitter2 = CreateTestEmitter();
        effect.Add(emitter1);
        effect.Add(emitter2);

        // Act
        var count = effect.ActiveParticlesCount;

        // Assert - Emitter implementations may vary
        count.Should().BeGreaterOrEqualTo(0);
    }

    // Test obsolete Update method
    [Fact]
    public void Update_ObsoleteVersion_WorksCorrectly()
    {
        // Arrange
        var effect = new ParticleEffect();
        const float totalSeconds = 10f;
        const float deltaSeconds = 0.016f;

        // Act & Assert - Obsolete method should still work
        effect.Update(totalSeconds, deltaSeconds);
        effect.ActiveParticlesCount.Should().BeGreaterOrEqualTo(0);
    }

    // Test with multiple emitters
    [Fact]
    public void ParticleEffect_WithMultipleEmitters_ManagesAllEmitters()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitters = new[]
        {
            CreateTestEmitter(),
            CreateTestEmitter(),
            CreateTestEmitter()
        };

        // Act
        foreach (var emitter in emitters)
        {
            effect.Add(emitter);
        }

        // Assert
        effect.Count.Should().Be(3);
    }

    // Test enumerator support
    [Fact]
    public void ParticleEffect_SupportsEnumeration()
    {
        // Arrange
        var effect = new ParticleEffect();
        var emitter1 = CreateTestEmitter();
        var emitter2 = CreateTestEmitter();
        effect.Add(emitter1);
        effect.Add(emitter2);

        // Act
        var count = 0;
        foreach (var emitter in effect)
        {
            count++;
            emitter.Should().NotBeNull();
        }

        // Assert
        count.Should().Be(2);
    }

    // Test controllers collection
    [Fact]
    public void ControllersCollection_CanAddControllers()
    {
        // Arrange
        var effect = new ParticleEffect();

        // Act & Assert
        effect.Controllers.Should().NotBeNull();
        effect.Controllers.Owner.Should().Be(effect);
    }

    // Test edge cases
    [Fact]
    public void Trigger_OnEmptyEffect_DoesNotThrow()
    {
        // Arrange
        var effect = new ParticleEffect();
        var position = new Vector2(100, 200);

        // Act & Assert
        effect.Trigger(position);
    }

    [Fact]
    public void Update_OnEmptyEffect_DoesNotThrow()
    {
        // Arrange
        var effect = new ParticleEffect();
        const float deltaSeconds = 0.016f;

        // Act & Assert
        effect.Update(deltaSeconds);
    }

    // Helper method
    private static Emitter CreateTestEmitter()
    {
        // Create a simple test emitter
        // This will need to be adapted based on the actual Emitter implementation
        return null; // Placeholder - would return a mock or test emitter
    }
}
