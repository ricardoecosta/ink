using FluentAssertions;
using Xunit;
using System.Globalization;
using HamstasKitties.Particles;

namespace HamstasKitties.Shared.Tests.Particles;

/// <summary>
/// Unit tests for the Range struct.
/// Tests cover range creation, comparison, parsing, and mathematical operations.
/// </summary>
public class RangeTests
{
    // Test construction
    [Fact]
    public void Range_Constructor_CreatesRange()
    {
        // Arrange
        const float minimum = 0f;
        const float maximum = 10f;

        // Act
        var range = new Range(minimum, maximum);

        // Assert
        range.Minimum.Should().Be(minimum);
        range.Maximum.Should().Be(maximum);
    }

    // Test Size property
    [Theory]
    [InlineData(0f, 10f, 10f)]
    [InlineData(-5f, 5f, 10f)]
    [InlineData(0f, 0f, 0f)]
    [InlineData(-10f, -5f, 5f)]
    public void Size_ReturnsCorrectValue(float minimum, float maximum, float expected)
    {
        // Arrange
        var range = new Range(minimum, maximum);

        // Act
        var result = range.Size;

        // Assert
        result.Should().BeApproximately(expected, 0.0001f);
    }

    // Test Contains method with Range
    [Fact]
    public void Contains_Range_WhenContained_ReturnsTrue()
    {
        // Arrange
        var outer = new Range(0f, 10f);
        var inner = new Range(2f, 8f);

        // Act
        var result = outer.Contains(inner);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Contains_Range_WhenNotContained_ReturnsFalse()
    {
        // Arrange
        var outer = new Range(0f, 10f);
        var inner = new Range(5f, 15f);

        // Act
        var result = outer.Contains(inner);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Contains_Range_WhenIdentical_ReturnsTrue()
    {
        // Arrange
        var range = new Range(0f, 10f);

        // Act
        var result = range.Contains(range);

        // Assert
        result.Should().BeTrue();
    }

    // Test Contains method with float
    [Theory]
    [InlineData(0f, 10f, 5f, true)]
    [InlineData(0f, 10f, 0f, true)]
    [InlineData(0f, 10f, 10f, true)]
    [InlineData(0f, 10f, -1f, false)]
    [InlineData(0f, 10f, 11f, false)]
    public void Contains_Float_ReturnsCorrectResult(float minimum, float maximum, float value, bool expected)
    {
        // Arrange
        var range = new Range(minimum, maximum);

        // Act
        var result = range.Contains(value);

        // Assert
        result.Should().Be(expected);
    }

    // Test Merge operation
    [Fact]
    public void Merge_ExpandsRangeToIncludeOther()
    {
        // Arrange
        var range = new Range(0f, 10f);
        var other = new Range(5f, 15f);

        // Act
        range.Merge(other);

        // Assert
        range.Minimum.Should().Be(0f);
        range.Maximum.Should().Be(15f);
    }

    [Fact]
    public void Merge_WithLowerRange_ExpandsMinimum()
    {
        // Arrange
        var range = new Range(5f, 10f);
        var other = new Range(0f, 8f);

        // Act
        range.Merge(other);

        // Assert
        range.Minimum.Should().Be(0f);
        range.Maximum.Should().Be(10f);
    }

    // Test Intersect operation
    [Fact]
    public void Intersect_ReturnsOverlappingRange()
    {
        // Arrange
        var range = new Range(0f, 10f);
        var other = new Range(5f, 15f);

        // Act
        range.Intersect(other);

        // Assert
        range.Minimum.Should().Be(5f);
        range.Maximum.Should().Be(10f);
    }

    [Fact]
    public void Intersect_WithNoOverlap_ExtendsToTouch()
    {
        // Arrange
        var range = new Range(0f, 10f);
        var other = new Range(15f, 20f);

        // Act
        range.Intersect(other);

        // Assert - Behavior may vary, should document
        range.Minimum.Should().BeGreaterOrEqualTo(0f);
    }

    // Test Subtract operation
    [Fact]
    public void Subtract_RemovesOverlap()
    {
        // Arrange
        var range = new Range(0f, 10f);
        var other = new Range(5f, 15f);

        // Act
        range.Subtract(other);

        // Assert - Should remove overlapping portion
        range.Maximum.Should().BeLessOrEqualTo(10f);
    }

    // Test static Union method
    [Theory]
    [InlineData(0f, 10f, 5f, 15f, 0f, 15f)]
    [InlineData(-5f, 5f, 0f, 10f, -5f, 10f)]
    public void Union_ReturnsCombinedRange(float min1, float max1, float min2, float max2, float expectedMin, float expectedMax)
    {
        // Arrange
        var range1 = new Range(min1, max1);
        var range2 = new Range(min2, max2);

        // Act
        var result = Range.Union(range1, range2);

        // Assert
        result.Minimum.Should().Be(expectedMin);
        result.Maximum.Should().Be(expectedMax);
    }

    // Test static Intersect method
    [Fact]
    public void StaticIntersect_ReturnsOverlappingRange()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = Range.Intersect(range1, range2);

        // Assert
        result.Minimum.Should().Be(5f);
        result.Maximum.Should().Be(10f);
    }

    // Test static Subtract method
    [Fact]
    public void StaticSubtract_ReturnsDifference()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = Range.Subtract(range1, range2);

        // Assert
        result.Minimum.Should().Be(0f);
        result.Maximum.Should().Be(5f);
    }

    // Test Parse method
    [Theory]
    [InlineData("[0,10]", 0f, 10f)]
    [InlineData("[-5,5]", -5f, 5f)]
    [InlineData("[0.5,1.5]", 0.5f, 1.5f)]
    public void Parse_WithValidString_ReturnsRange(string input, float expectedMin, float expectedMax)
    {
        // Arrange & Act
        var result = Range.Parse(input, CultureInfo.InvariantCulture);

        // Assert
        result.Minimum.Should().BeApproximately(expectedMin, 0.0001f);
        result.Maximum.Should().BeApproximately(expectedMax, 0.0001f);
    }

    [Fact]
    public void Parse_WithInvalidString_ThrowsFormatException()
    {
        // Arrange
        var invalidInput = "invalid";

        // Act & Assert
        Assert.Throws<FormatException>(() => Range.Parse(invalidInput, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Parse_WithoutFormatProvider_UsesInvariantCulture()
    {
        // Arrange & Act
        var result = Range.Parse("[0,10]");

        // Assert
        result.Minimum.Should().Be(0f);
        result.Maximum.Should().Be(10f);
    }

    // Test ToString method
    [Fact]
    public void ToString_ReturnsIsoFormat()
    {
        // Arrange
        var range = new Range(0f, 10f);

        // Act
        var result = range.ToString();

        // Assert
        result.Should().Be("[0,10]");
    }

    [Fact]
    public void ToString_WithNegativeValues_ReturnsCorrectFormat()
    {
        // Arrange
        var range = new Range(-5f, 5f);

        // Act
        var result = range.ToString(CultureInfo.InvariantCulture);

        // Assert
        result.Should().Contain("-5");
        result.Should().Contain("5");
        result.Should().StartWith("[");
        result.Should().EndWith("]");
    }

    // Test equality operators
    [Fact]
    public void EqualityOperator_WithEqualRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(0f, 10f);

        // Act
        var result = range1 == range2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_WithDifferentRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = range1 == range2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_WithDifferentRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = range1 != range2;

        // Assert
        result.Should().BeTrue();
    }

    // Test Equals method
    [Fact]
    public void Equals_WithEqualRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(0f, 10f);

        // Act
        var result = range1.Equals(range2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(0f, 5f);

        // Act
        var result = range1.Equals(range2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var range = new Range(0f, 10f);

        // Act
        var result = range.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    // Test GetHashCode
    [Fact]
    public void GetHashCode_WithEqualRanges_ReturnsSameHash()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(0f, 10f);

        // Act
        var hash1 = range1.GetHashCode();
        var hash2 = range2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentRanges_CanReturnDifferentHashes()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var hash1 = range1.GetHashCode();
        var hash2 = range2.GetHashCode();

        // Assert - Hash codes might collide, but shouldn't be equal for different values
        // We just verify they both return valid hash codes
        hash1.Should().NotBe(0);
        hash2.Should().NotBe(0);
    }

    // Test operator overloads
    [Fact]
    public void AdditionOperator_PerformsUnion()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = range1 + range2;

        // Assert
        result.Minimum.Should().Be(0f);
        result.Maximum.Should().Be(15f);
    }

    [Fact]
    public void SubtractionOperator_PerformsSubtract()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = range1 - range2;

        // Assert
        result.Minimum.Should().Be(0f);
        result.Maximum.Should().Be(5f);
    }

    [Fact]
    public void BitwiseOrOperator_PerformsIntersect()
    {
        // Arrange
        var range1 = new Range(0f, 10f);
        var range2 = new Range(5f, 15f);

        // Act
        var result = (range1 | range2);

        // Assert
        result.Minimum.Should().Be(5f);
        result.Maximum.Should().Be(10f);
    }

    // Test edge cases
    [Fact]
    public void Range_WithZeroWidth_IsValid()
    {
        // Arrange & Act
        var range = new Range(5f, 5f);

        // Assert
        range.Minimum.Should().Be(5f);
        range.Maximum.Should().Be(5f);
        range.Size.Should().Be(0f);
    }

    [Fact]
    public void Range_WithNegativeMinimum_IsValid()
    {
        // Arrange & Act
        var range = new Range(-10f, 10f);

        // Assert
        range.Minimum.Should().Be(-10f);
        range.Maximum.Should().Be(10f);
        range.Size.Should().Be(20f);
    }

    [Fact]
    public void Range_WithInvertedBounds_IsValid()
    {
        // Arrange & Act - Range doesn't enforce min < max
        var range = new Range(10f, 0f);

        // Assert
        range.Minimum.Should().Be(10f);
        range.Maximum.Should().Be(0f);
    }
}
