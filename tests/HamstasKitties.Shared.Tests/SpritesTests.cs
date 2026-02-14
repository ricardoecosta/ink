using Xunit;
using FluentAssertions;
using HamstasKitties.Sprites;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moq;

namespace HamstasKitties.Shared.Tests
{
    public class ProgressBarTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);
            var position = new Vector2(10, 20);
            var size = new Point(100, 20);
            var startColor = Color.Red;
            var endColor = Color.Blue;
            var borderColor = Color.White;
            var borderThickness = 2;

            // Act
            var progressBar = new ProgressBar(mockParentLayer.Object, position, size, startColor, endColor, borderColor, borderThickness);

            // Assert
            progressBar.Should().NotBeNull();
            progressBar.Progress.Should().Be(0);
        }

        [Fact]
        public void Progress_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);
            var progressBar = new ProgressBar(mockParentLayer.Object, Vector2.Zero, new Point(100, 20), Color.Red, Color.Blue, Color.White, 2);

            // Act
            progressBar.Progress = 50f;

            // Assert
            progressBar.Progress.Should().Be(50f);
        }
    }

    public class ComboTextTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);
            var position = new Vector2(10, 20);
            var multiplier = 5;
            var mockFont = new Mock<SpriteFont>();

            // Act
            var comboText = new ComboText(mockParentLayer.Object, position, multiplier, mockFont.Object);

            // Assert
            comboText.Should().NotBeNull();
        }
    }

    public class RisingUpPointsTextTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);
            var mockBlock = new Mock<Block>(null, Block.BlockTypes.Block1, 0, null);
            var points = 100L;
            var mockFont = new Mock<SpriteFont>();

            // Act
            var pointsText = new RisingUpPointsText(mockParentLayer.Object, mockBlock.Object, points);

            // Assert
            pointsText.Should().NotBeNull();
        }
    }

    public class ZoomInFadeOutTextTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);
            var position = new Vector2(10, 20);
            var text = "Test Text";
            var mockFont = new Mock<SpriteFont>();

            // Act
            var zoomText = new ZoomInFadeOutText(mockParentLayer.Object, position, text, mockFont.Object);

            // Assert
            zoomText.Should().NotBeNull();
        }
    }

    public class GoldenHamstaSparkleTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);
            var mockBlock = new Mock<Block>(null, Block.BlockTypes.GoldenBlock, 0, null);

            // Act
            var sparkle = new GoldenHamstaSparkle(mockParentLayer.Object, mockBlock.Object);

            // Assert
            sparkle.Should().NotBeNull();
        }
    }

    public class RandomPairOfBlinkingEyesTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockParentLayer = new Mock<Layer>(null, LayerTypes.Interactive, Vector2.Zero, 0, false);

            // Act
            var eyes = new RandomPairOfBlinkingEyes(mockParentLayer.Object);

            // Assert
            eyes.Should().NotBeNull();
        }
    }
}
