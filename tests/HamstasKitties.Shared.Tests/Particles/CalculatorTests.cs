using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using HamstasKitties.Particles;

namespace HamstasKitties.Shared.Tests.Particles;

/// <summary>
/// Unit tests for the Calculator static class.
/// Tests cover mathematical functions, clamping, interpolation, and utility methods.
/// </summary>
public class CalculatorTests
{
    // Test constants
    [Fact]
    public void Pi_HasExpectedValue()
    {
        // Arrange & Act & Assert
        Calculator.Pi.Should().BeApproximately(3.141593f, 0.0001f);
    }

    [Fact]
    public void TwoPi_HasExpectedValue()
    {
        // Arrange & Act & Assert
        Calculator.TwoPi.Should().BeApproximately(6.283185f, 0.0001f);
    }

    [Fact]
    public void PiOver2_HasExpectedValue()
    {
        // Arrange & Act & Assert
        Calculator.PiOver2.Should().BeApproximately(1.570796f, 0.0001f);
    }

    [Fact]
    public void PiOver4_HasExpectedValue()
    {
        // Arrange & Act & Assert
        Calculator.PiOver4.Should().BeApproximately(0.7853982f, 0.0001f);
    }

    // Test Clamp methods
    [Theory]
    [InlineData(5f, 0f, 10f, 5f)]
    [InlineData(-5f, 0f, 10f, 0f)]
    [InlineData(15f, 0f, 10f, 10f)]
    [InlineData(0f, 0f, 10f, 0f)]
    [InlineData(10f, 0f, 10f, 10f)]
    public void Clamp_ReturnsCorrectValue(float value, float min, float max, float expected)
    {
        // Arrange & Act
        var result = Calculator.Clamp(value, min, max);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Clamp_WithRange_ReturnsCorrectValue()
    {
        // Arrange
        var range = new Range(0f, 10f);

        // Act
        var result = Calculator.Clamp(15f, range);

        // Assert
        result.Should().Be(10f);
    }

    [Theory]
    [InlineData(5f, 0f, 10f, 5f)]
    [InlineData(-5f, 0f, 10f, 0f)]
    [InlineData(15f, 0f, 10f, 10f)]
    public void Clamp_Ref_ModifiesValueCorrectly(float value, float min, float max, float expected)
    {
        // Arrange
        var testValue = value;

        // Act
        Calculator.Clamp(ref testValue, min, max);

        // Assert
        testValue.Should().Be(expected);
    }

    // Test generic Clamp
    [Theory]
    [InlineData(5, 0, 10, 5)]
    [InlineData(-5, 0, 10, 0)]
    [InlineData(15, 0, 10, 10)]
    public void_Clamp_Generic_ReturnsCorrectValue(int value, int min, int max, int expected)
    {
        // Arrange & Act
        var result = Calculator.Clamp(value, min, max);

        // Assert
        result.Should().Be(expected);
    }

    // Test Wrap methods
    [Theory]
    [InlineData(5f, 0f, 10f, 5f)]
    [InlineData(-5f, 0f, 10f, 5f)]
    [InlineData(15f, 0f, 10f, 5f)]
    [InlineData(20f, 0f, 10f, 0f)]
    public void Wrap_ReturnsWrappedValue(float value, float min, float max, float expected)
    {
        // Arrange & Act
        var result = Calculator.Wrap(value, min, max);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Wrap_WithRange_ReturnsWrappedValue()
    {
        // Arrange
        var range = new Range(0f, 10f);

        // Act
        var result = Calculator.Wrap(15f, range);

        // Assert
        result.Should().Be(5f);
    }

    // Test LinearInterpolate methods
    [Theory]
    [InlineData(0f, 10f, 0f, 0f)]
    [InlineData(0f, 10f, 0.5f, 5f)]
    [InlineData(0f, 10f, 1f, 10f)]
    public void LinearInterpolate_ReturnsCorrectValue(float value1, float value2, float amount, float expected)
    {
        // Arrange & Act
        var result = Calculator.LinearInterpolate(value1, value2, amount);

        // Assert
        result.Should().BeApproximately(expected, 0.0001f);
    }

    [Fact]
    public void LinearInterpolate_WithThreeValues_ReturnsCorrectValue()
    {
        // Arrange
        const float value1 = 0f;
        const float value2 = 5f;
        const float value2Position = 0.5f;
        const float value3 = 10f;

        // Act & Assert
        Calculator.LinearInterpolate(value1, value2, value2Position, value3, 0.25f)
            .Should().BeApproximately(2.5f, 0.0001f);
        Calculator.LinearInterpolate(value1, value2, value2Position, value3, 0.5f)
            .Should().BeApproximately(5f, 0.0001f);
        Calculator.LinearInterpolate(value1, value2, value2Position, value3, 0.75f)
            .Should().BeApproximately(7.5f, 0.0001f);
    }

    // Test CubicInterpolate
    [Theory]
    [InlineData(0f, 10f, 0f, 0f)]
    [InlineData(0f, 10f, 0.5f, 5f)]
    [InlineData(0f, 10f, 1f, 10f)]
    public void CubicInterpolate_ReturnsCorrectValue(float value1, float value2, float amount, float expected)
    {
        // Arrange & Act
        var result = Calculator.CubicInterpolate(value1, value2, amount);

        // Assert
        result.Should().BeApproximately(expected, 0.001f);
    }

    // Test Max methods
    [Theory]
    [InlineData(5f, 10f, 10f)]
    [InlineData(10f, 5f, 10f)]
    [InlineData(5f, 5f, 5f)]
    public void Max_TwoValues_ReturnsLarger(float value1, float value2, float expected)
    {
        // Arrange & Act
        var result = Calculator.Max(value1, value2);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5f, 10f, 15f, 15f)]
    [InlineData(15f, 10f, 5f, 15f)]
    [InlineData(10f, 15f, 5f, 15f)]
    public void Max_ThreeValues_ReturnsLargest(float value1, float value2, float value3, float expected)
    {
        // Arrange & Act
        var result = Calculator.Max(value1, value2, value3);

        // Assert
        result.Should().Be(expected);
    }

    // Test Min methods
    [Theory]
    [InlineData(5f, 10f, 5f)]
    [InlineData(10f, 5f, 5f)]
    [InlineData(5f, 5f, 5f)]
    public void Min_TwoValues_ReturnsSmaller(float value1, float value2, float expected)
    {
        // Arrange & Act
        var result = Calculator.Min(value1, value2);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5f, 10f, 15f, 5f)]
    [InlineData(15f, 10f, 5f, 5f)]
    [InlineData(10f, 5f, 15f, 5f)]
    public void Min_ThreeValues_ReturnsSmallest(float value1, float value2, float value3, float expected)
    {
        // Arrange & Act
        var result = Calculator.Min(value1, value2, value3);

        // Assert
        result.Should().Be(expected);
    }

    // Test Abs method
    [Theory]
    [InlineData(5f, 5f)]
    [InlineData(-5f, 5f)]
    [InlineData(0f, 0f)]
    public void Abs_ReturnsAbsoluteValue(float value, float expected)
    {
        // Arrange & Act
        var result = Calculator.Abs(value);

        // Assert
        result.Should().Be(expected);
    }

    // Test trigonometric methods
    [Fact]
    public void Sin_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Sin(0f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    [Fact]
    public void Cos_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Cos(0f);

        // Assert
        result.Should().BeApproximately(1f, 0.0001f);
    }

    [Fact]
    public void Tan_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Tan(0f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    [Fact]
    public void Atan2_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Atan2(0f, 1f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    // Test inverse trigonometric methods
    [Fact]
    public void Acos_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Acos(1f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    [Fact]
    public void Asin_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Asin(0f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    [Fact]
    public void Atan_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Atan(0f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    // Test hyperbolic methods
    [Fact]
    public void Sinh_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Sinh(0f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    [Fact]
    public void Cosh_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Cosh(0f);

        // Assert
        result.Should().BeApproximately(1f, 0.0001f);
    }

    [Fact]
    public void Tanh_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Tanh(0f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    // Test other math methods
    [Fact]
    public void Sqrt_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Sqrt(16f);

        // Assert
        result.Should().BeApproximately(4f, 0.0001f);
    }

    [Fact]
    public void Pow_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Pow(2f, 3f);

        // Assert
        result.Should().BeApproximately(8f, 0.0001f);
    }

    [Fact]
    public void Log_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = Calculator.Log(1f);

        // Assert
        result.Should().BeApproximately(0f, 0.0001f);
    }

    // Test edge cases
    [Fact]
    public void Clamp_WithMinGreaterThanMax_WorksCorrectly()
    {
        // Arrange
        const float value = 5f;
        const float min = 10f;
        const float max = 0f;

        // Act
        var result = Calculator.Clamp(value, min, max);

        // Assert - Should clamp to one of the bounds
        result.Should().BeOneOf(min, max);
    }

    [Fact]
    public void Wrap_WithValuesAtBounds_ReturnsCorrectValue()
    {
        // Arrange
        const float value = 10f;
        const float min = 0f;
        const float max = 10f;

        // Act
        var result = Calculator.Wrap(value, min, max);

        // Assert
        result.Should().Be(value);
    }
}
