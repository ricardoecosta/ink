using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using System.Globalization;
using HamstasKitties.Particles;

namespace HamstasKitties.Shared.Tests.Particles;

/// <summary>
/// Unit tests for the VariableFloat3 struct.
/// Tests cover 3D vector random value generation with variation, sampling, and type conversions.
/// </summary>
public class VariableFloat3Tests
{
    // Test construction and default values
    [Fact]
    public void VariableFloat3_DefaultConstruction_HasZeroVectors()
    {
        // Arrange & Act
        var variableFloat3 = new VariableFloat3();

        // Assert
        variableFloat3.Value.Should().Be(Vector3.Zero);
        variableFloat3.Variation.Should().Be(Vector3.Zero);
    }

    [Fact]
    public void VariableFloat3_ExplicitConstruction_HasSpecifiedValues()
    {
        // Arrange
        var value = new Vector3(1f, 2f, 3f);
        var variation = new Vector3(0.1f, 0.2f, 0.3f);

        // Act
        var variableFloat3 = new VariableFloat3 { Value = value, Variation = variation };

        // Assert
        variableFloat3.Value.Should().Be(value);
        variableFloat3.Variation.Should().Be(variation);
    }

    // Test Sample method
    [Fact]
    public void Sample_WithZeroVariation_ReturnsExactValue()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = Vector3.Zero
        };

        // Act
        var result = variableFloat3.Sample();

        // Assert
        result.X.Should().Be(10f);
        result.Y.Should().Be(20f);
        result.Z.Should().Be(30f);
    }

    [Fact]
    public void Sample_WithVariation_ReturnsValueWithinRange()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = new Vector3(2f, 3f, 4f)
        };

        // Act
        for (int i = 0; i < 50; i++)
        {
            var result = variableFloat3.Sample();

            // Assert - Each component should be within [Value - Variation, Value + Variation]
            result.X.Should().BeGreaterOrEqualTo(8f);
            result.X.Should().BeLessOrEqualTo(12f);

            result.Y.Should().BeGreaterOrEqualTo(17f);
            result.Y.Should().BeLessOrEqualTo(23f);

            result.Z.Should().BeGreaterOrEqualTo(26f);
            result.Z.Should().BeLessOrEqualTo(34f);
        }
    }

    [Fact]
    public void Sample_WithPartialVariation_OnlyVariesSpecifiedComponents()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = new Vector3(2f, 0f, 0f) // Only X varies
        };

        // Act
        var results = new System.Collections.Generic.HashSet<float>();
        for (int i = 0; i < 20; i++)
        {
            var result = variableFloat3.Sample();
            results.Add(result.X);

            // Y and Z should always be exact
            result.Y.Should().Be(20f);
            result.Z.Should().Be(30f);
        }

        // Assert - X should vary
        results.Count.Should().BeGreaterThan(1, "X component should vary");
    }

    [Fact]
    public void Sample_WithDifferentVariationsPerComponent_UsesEachVariation()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(100f, 100f, 100f),
            Variation = new Vector3(10f, 20f, 30f)
        };

        // Act
        for (int i = 0; i < 20; i++)
        {
            var result = variableFloat3.Sample();

            // Assert - Each component should be within its own range
            result.X.Should().BeGreaterOrEqualTo(90f);
            result.X.Should().BeLessOrEqualTo(110f);

            result.Y.Should().BeGreaterOrEqualTo(80f);
            result.Y.Should().BeLessOrEqualTo(120f);

            result.Z.Should().BeGreaterOrEqualTo(70f);
            result.Z.Should().BeLessOrEqualTo(130f);
        }
    }

    // Test implicit conversion from Vector3
    [Fact]
    public void ImplicitConversion_FromVector3_CreatesVariableFloat3WithZeroVariation()
    {
        // Arrange
        var value = new Vector3(1f, 2f, 3f);

        // Act
        VariableFloat3 variableFloat3 = value;

        // Assert
        variableFloat3.Value.Should().Be(value);
        variableFloat3.Variation.Should().Be(Vector3.Zero);
    }

    [Theory]
    [InlineData(0f, 0f, 0f)]
    [InlineData(1f, 2f, 3f)]
    [InlineData(-1f, -2f, -3f)]
    [InlineData(3.14159f, 2.71828f, 1.41421f)]
    public void ImplicitConversion_FromVector3WithVariousValues_WorksCorrectly(float x, float y, float z)
    {
        // Arrange
        var value = new Vector3(x, y, z);

        // Act
        VariableFloat3 variableFloat3 = value;

        // Assert
        variableFloat3.Value.Should().Be(value);
        variableFloat3.Variation.Should().Be(Vector3.Zero);
    }

    // Test implicit conversion to Vector3
    [Fact]
    public void ImplicitConversion_ToVector3_WithZeroVariation_ReturnsValue()
    {
        // Arrange
        var value = new Vector3(10f, 20f, 30f);
        var variableFloat3 = new VariableFloat3
        {
            Value = value,
            Variation = Vector3.Zero
        };

        // Act
        Vector3 result = variableFloat3;

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void ImplicitConversion_ToVector3_WithVariation_ReturnsRandomizedValue()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = new Vector3(2f, 3f, 4f)
        };
        var results = new System.Collections.Generic.HashSet<Vector3>();

        // Act
        for (int i = 0; i < 30; i++)
        {
            Vector3 result = variableFloat3;
            results.Add(result);
        }

        // Assert - With variation, we should get different values
        results.Count.Should().BeGreaterThan(1, "Should generate different values with variation");
    }

    // Test Equals method
    [Fact]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        // Arrange
        var vf1 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };
        var vf2 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };

        // Act
        var result = vf1.Equals(vf2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var vf1 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };
        var vf2 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.4f) // Different Z variation
        };

        // Act
        var result = vf1.Equals(vf2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };

        // Act
        var result = variableFloat3.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };

        // Act
        var result = variableFloat3.Equals("not a VariableFloat3");

        // Assert
        result.Should().BeFalse();
    }

    // Test GetHashCode
    [Fact]
    public void GetHashCode_WithSameValues_ReturnsSameHash()
    {
        // Arrange
        var vf1 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };
        var vf2 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };

        // Act
        var hash1 = vf1.GetHashCode();
        var hash2 = vf2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    // Test ToString
    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(1f, 2f, 3f),
            Variation = new Vector3(0.1f, 0.2f, 0.3f)
        };

        // Act
        var result = variableFloat3.ToString();

        // Assert
        result.Should().Contain("Value");
        result.Should().Contain("Variation");
    }

    // Test edge cases
    [Fact]
    public void Sample_WithVerySmallVariation_ReturnsApproximateValue()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = new Vector3(0.0001f, 0.0001f, 0.0001f)
        };

        // Act
        for (int i = 0; i < 10; i++)
        {
            var result = variableFloat3.Sample();

            // Assert - Should be very close to original values
            result.X.Should().BeApproximately(10f, 0.0002f);
            result.Y.Should().BeApproximately(20f, 0.0002f);
            result.Z.Should().BeApproximately(30f, 0.0002f);
        }
    }

    [Fact]
    public void Sample_WithNegativeVariation_CanProduceNegativeValues()
    {
        // Arrange
        var variableFloat3 = new VariableFloat3
        {
            Value = new Vector3(0f, 0f, 0f),
            Variation = new Vector3(10f, 10f, 10f)
        };
        var foundNegativeX = false;
        var foundNegativeY = false;
        var foundNegativeZ = false;

        // Act
        for (int i = 0; i < 100; i++)
        {
            var result = variableFloat3.Sample();
            if (result.X < 0f) foundNegativeX = true;
            if (result.Y < 0f) foundNegativeY = true;
            if (result.Z < 0f) foundNegativeZ = true;
        }

        // Assert
        foundNegativeX.Should().BeTrue("Should find negative X values");
        foundNegativeY.Should().BeTrue("Should find negative Y values");
        foundNegativeZ.Should().BeTrue("Should find negative Z values");
    }

    // Test struct behavior
    [Fact]
    public void VariableFloat3_IsStruct()
    {
        // Arrange & Act & Assert
        var type = typeof(VariableFloat3);
        type.IsValueType.Should().BeTrue();
    }

    // Test multiple instances
    [Fact]
    public void MultipleVariableFloat3s_AreIndependent()
    {
        // Arrange
        var vf1 = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = new Vector3(2f, 3f, 4f)
        };
        var vf2 = new VariableFloat3
        {
            Value = new Vector3(100f, 200f, 300f),
            Variation = new Vector3(20f, 30f, 40f)
        };

        // Act
        var result1 = vf1.Sample();
        var result2 = vf2.Sample();

        // Assert - Results should be in completely different ranges
        result1.X.Should().BeLessThan(20f);
        result2.X.Should().BeGreaterThan(80f);
    }

    // Test with common use cases
    [Fact]
    public void VariableFloat3_CanRepresentPositionWithVariation()
    {
        // Arrange - 3D position with ±1 unit variation in each axis
        var position = new VariableFloat3
        {
            Value = new Vector3(10f, 20f, 30f),
            Variation = new Vector3(1f, 1f, 1f)
        };

        // Act
        var result = position.Sample();

        // Assert
        result.X.Should().BeGreaterOrEqualTo(9f);
        result.X.Should().BeLessOrEqualTo(11f);

        result.Y.Should().BeGreaterOrEqualTo(19f);
        result.Y.Should().BeLessOrEqualTo(21f);

        result.Z.Should().BeGreaterOrEqualTo(29f);
        result.Z.Should().BeLessOrEqualTo(31f);
    }

    [Fact]
    public void VariableFloat3_CanRepresentVelocityWithVariation()
    {
        // Arrange - Velocity with variation for randomness
        var velocity = new VariableFloat3
        {
            Value = new Vector3(5f, 0f, -2f),
            Variation = new Vector3(0.5f, 0.5f, 0.5f)
        };

        // Act
        for (int i = 0; i < 20; i++)
        {
            var result = velocity.Sample();

            // Assert
            result.X.Should().BeGreaterOrEqualTo(4.5f);
            result.X.Should().BeLessOrEqualTo(5.5f);

            result.Y.Should().BeGreaterOrEqualTo(-0.5f);
            result.Y.Should().BeLessOrEqualTo(0.5f);

            result.Z.Should().BeGreaterOrEqualTo(-2.5f);
            result.Z.Should().BeLessOrEqualTo(-1.5f);
        }
    }

    [Fact]
    public void VariableFloat3_CanRepresentScaleWithVariation()
    {
        // Arrange - Uniform scale with ±10% variation
        var scale = new VariableFloat3
        {
            Value = new Vector3(1f, 1f, 1f),
            Variation = new Vector3(0.1f, 0.1f, 0.1f)
        };

        // Act
        for (int i = 0; i < 20; i++)
        {
            var result = scale.Sample();

            // Assert - All components should be positive
            result.X.Should().BeGreaterThan(0f);
            result.Y.Should().BeGreaterThan(0f);
            result.Z.Should().BeGreaterThan(0f);

            // And within reasonable bounds
            result.X.Should().BeLessOrEqualTo(1.1f);
            result.Y.Should().BeLessOrEqualTo(1.1f);
            result.Z.Should().BeLessOrEqualTo(1.1f);
        }
    }

    [Fact]
    public void VariableFloat3_CanRepresentColorWithVariation()
    {
        // Arrange - RGB color with variation
        var color = new VariableFloat3
        {
            Value = new Vector3(1f, 0.5f, 0f), // Orange
            Variation = new Vector3(0.1f, 0.1f, 0.1f)
        };

        // Act
        for (int i = 0; i < 20; i++)
        {
            var result = color.Sample();

            // Assert - RGB values should be in [0, 1] range
            result.X.Should().BeGreaterOrThanOrEqualTo(0f);
            result.X.Should().BeLessOrThanOrEqualTo(1f);

            result.Y.Should().BeGreaterOrThanOrEqualTo(0f);
            result.Y.Should().BeLessOrThanOrEqualTo(1f);

            result.Z.Should().BeGreaterOrThanOrEqualTo(0f);
            result.Z.Should().BeLessOrThanOrEqualTo(1f);
        }
    }
}
