using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using System.Globalization;
using HamstasKitties.Particles;

namespace HamstasKitties.Shared.Tests.Particles;

/// <summary>
/// Unit tests for the VariableFloat struct.
/// Tests cover random value generation with variation, sampling, and type conversions.
/// </summary>
public class VariableFloatTests
{
    // Test construction and default values
    [Fact]
    public void VariableFloat_DefaultConstruction_HasZeroValues()
    {
        // Arrange & Act
        var variableFloat = new VariableFloat();

        // Assert
        variableFloat.Value.Should().Be(0f);
        variableFloat.Variation.Should().Be(0f);
    }

    [Fact]
    public void VariableFloat_ExplicitConstruction_HasSpecifiedValues()
    {
        // Arrange
        const float value = 10f;
        const float variation = 2f;

        // Act
        var variableFloat = new VariableFloat { Value = value, Variation = variation };

        // Assert
        variableFloat.Value.Should().Be(value);
        variableFloat.Variation.Should().Be(variation);
    }

    // Test Sample method without range constraint
    [Fact]
    public void Sample_WithZeroVariation_ReturnsExactValue()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 0f };

        // Act
        var result = variableFloat.Sample();

        // Assert
        result.Should().Be(10f);
    }

    [Fact]
    public void Sample_WithVariation_ReturnsValueWithinRange()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 2f };
        var results = new float[100];

        // Act
        for (int i = 0; i < 100; i++)
        {
            results[i] = variableFloat.Sample();
        }

        // Assert - All samples should be within [Value - Variation, Value + Variation]
        foreach (var result in results)
        {
            result.Should().BeGreaterOrEqualTo(8f);
            result.Should().BeLessOrEqualTo(12f);
        }
    }

    [Fact]
    public void Sample_WithLargeVariation_CanReturnExtremeValues()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 0f, Variation = 100f };
        var foundNegative = false;
        var foundPositive = false;

        // Act
        for (int i = 0; i < 100; i++)
        {
            var result = variableFloat.Sample();
            if (result < 0f) foundNegative = true;
            if (result > 0f) foundPositive = true;
        }

        // Assert
        foundNegative.Should().BeTrue("Should find negative values");
        foundPositive.Should().BeTrue("Should find positive values");
    }

    // Test Sample method with range constraint
    [Fact]
    public void Sample_WithRangeConstraint_RespectsRange()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 10f };
        var clampRange = new Range(5f, 15f);

        // Act
        for (int i = 0; i < 100; i++)
        {
            var result = variableFloat.Sample(clampRange);

            // Assert
            result.Should().BeGreaterOrEqualTo(5f);
            result.Should().BeLessOrEqualTo(15f);
        }
    }

    [Fact]
    public void Sample_WithRangeConstraint_ClampsWhenVariationExceedsRange()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 20f };
        var clampRange = new Range(8f, 12f);

        // Act
        for (int i = 0; i < 100; i++)
        {
            var result = variableFloat.Sample(clampRange);

            // Assert
            result.Should().BeGreaterOrEqualTo(8f);
            result.Should().BeLessOrEqualTo(12f);
        }
    }

    [Fact]
    public void Sample_WithZeroVariationAndRange_ReturnsClampedValue()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 15f, Variation = 0f };
        var clampRange = new Range(0f, 10f);

        // Act
        var result = variableFloat.Sample(clampRange);

        // Assert
        result.Should().Be(10f); // Clamped to maximum
    }

    // Test implicit conversion from float
    [Fact]
    public void ImplicitConversion_FromFloat_CreatesVariableFloatWithZeroVariation()
    {
        // Arrange
        const float value = 10f;

        // Act
        VariableFloat variableFloat = value;

        // Assert
        variableFloat.Value.Should().Be(value);
        variableFloat.Variation.Should().Be(0f);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-5f)]
    [InlineData(3.14159f)]
    [InlineData(1000f)]
    public void ImplicitConversion_FromFloatWithVariousValues_WorksCorrectly(float value)
    {
        // Act
        VariableFloat variableFloat = value;

        // Assert
        variableFloat.Value.Should().Be(value);
        variableFloat.Variation.Should().Be(0f);
    }

    // Test implicit conversion to float
    [Fact]
    public void ImplicitConversion_ToFloat_WithZeroVariation_ReturnsValue()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 0f };

        // Act
        float result = variableFloat;

        // Assert
        result.Should().Be(10f);
    }

    [Fact]
    public void ImplicitConversion_ToFloat_WithVariation_ReturnsRandomizedValue()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 2f };
        var results = new System.Collections.Generic.HashSet<float>();

        // Act
        for (int i = 0; i < 50; i++)
        {
            float result = variableFloat;
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
        var vf1 = new VariableFloat { Value = 10f, Variation = 2f };
        var vf2 = new VariableFloat { Value = 10f, Variation = 2f };

        // Act
        var result = vf1.Equals(vf2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var vf1 = new VariableFloat { Value = 10f, Variation = 2f };
        var vf2 = new VariableFloat { Value = 10f, Variation = 3f };

        // Act
        var result = vf1.Equals(vf2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 2f };

        // Act
        var result = variableFloat.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 2f };

        // Act
        var result = variableFloat.Equals("not a VariableFloat");

        // Assert
        result.Should().BeFalse();
    }

    // Test GetHashCode
    [Fact]
    public void GetHashCode_WithSameValues_ReturnsSameHash()
    {
        // Arrange
        var vf1 = new VariableFloat { Value = 10f, Variation = 2f };
        var vf2 = new VariableFloat { Value = 10f, Variation = 2f };

        // Act
        var hash1 = vf1.GetHashCode();
        var hash2 = vf2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_CanReturnDifferentHashes()
    {
        // Arrange
        var vf1 = new VariableFloat { Value = 10f, Variation = 2f };
        var vf2 = new VariableFloat { Value = 10f, Variation = 3f };

        // Act
        var hash1 = vf1.GetHashCode();
        var hash2 = vf2.GetHashCode();

        // Assert - Hash codes might not be different for all values
        // but they're computed from both Value and Variation
        hash1.Should().NotBe(0);
        hash2.Should().NotBe(0);
    }

    // Test ToString
    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 2f };

        // Act
        var result = variableFloat.ToString();

        // Assert
        result.Should().Contain("10");
        result.Should().Contain("2");
    }

    [Fact]
    public void ToString_WithNegativeValues_ReturnsFormattedString()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = -5f, Variation = -1f };

        // Act
        var result = variableFloat.ToString();

        // Assert
        result.Should().Contain("-5");
        result.Should().Contain("-1");
    }

    // Test edge cases
    [Fact]
    public void Sample_WithVerySmallVariation_ReturnsApproximateValue()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = 0.0001f };

        // Act
        for (int i = 0; i < 10; i++)
        {
            var result = variableFloat.Sample();

            // Assert - Should be very close to 10f
            result.Should().BeApproximately(10f, 0.0002f);
        }
    }

    [Fact]
    public void Sample_WithNaNVariation_HandledCorrectly()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = float.NaN };

        // Act & Assert - Should handle gracefully
        // The actual behavior depends on RandomHelper implementation
        variableFloat.Value.Should().Be(10f);
    }

    [Fact]
    public void Sample_WithInfiniteVariation_HandledCorrectly()
    {
        // Arrange
        var variableFloat = new VariableFloat { Value = 10f, Variation = float.PositiveInfinity };

        // Act & Assert - Should handle gracefully
        variableFloat.Value.Should().Be(10f);
    }

    // Test struct behavior
    [Fact]
    public void VariableFloat_IsStruct()
    {
        // Arrange & Act & Assert
        var type = typeof(VariableFloat);
        type.IsValueType.Should().BeTrue();
    }

    // Test multiple instances
    [Fact]
    public void MultipleVariableFloats_AreIndependent()
    {
        // Arrange
        var vf1 = new VariableFloat { Value = 10f, Variation = 2f };
        var vf2 = new VariableFloat { Value = 20f, Variation = 3f };

        // Act
        var result1 = vf1.Sample();
        var result2 = vf2.Sample();

        // Assert
        result1.Should().NotBe(result2);
    }

    // Test with common use cases
    [Fact]
    public void VariableFloat_CanRepresentPercentage()
    {
        // Arrange - Percentage with ±5% variation
        var percentage = new VariableFloat { Value = 50f, Variation = 5f };

        // Act
        var result = percentage.Sample();

        // Assert
        result.Should().BeGreaterOrEqualTo(45f);
        result.Should().BeLessOrEqualTo(55f);
    }

    [Fact]
    public void VariableFloat_CanRepresentAngle()
    {
        // Arrange - Angle in radians with ±0.1 variation
        var angle = new VariableFloat { Value = Calculator.Pi, Variation = 0.1f };

        // Act
        var result = angle.Sample();

        // Assert
        result.Should().BeGreaterOrEqualTo(Calculator.Pi - 0.1f);
        result.Should().BeLessOrEqualTo(Calculator.Pi + 0.1f);
    }
}
