using System;
using Xunit;
using HamstasKitties.Social.Gaming;

namespace HamstasKitties.Tests.Social
{
    public class ScoreTests
    {
        [Fact]
        public void Score_HasCorrectProperties()
        {
            // Arrange & Act
            var score = new Score
            {
                Mode = 1,
                Rank = 42,
                PlayerName = "TestPlayer",
                ScoreResult = 12345.67,
                Country = "US",
                PhotoURL = "https://example.com/photo.jpg",
                IsCurrentUser = false
            };

            // Assert
            Assert.Equal(1, score.Mode);
            Assert.Equal((ulong)42, score.Rank);
            Assert.Equal("TestPlayer", score.PlayerName);
            Assert.Equal(12345.67, score.ScoreResult);
            Assert.Equal("US", score.Country);
            Assert.Equal("https://example.com/photo.jpg", score.PhotoURL);
            Assert.False(score.IsCurrentUser);
        }

        [Fact]
        public void Score_DefaultValuesAreCorrect()
        {
            // Arrange & Act
            var score = new Score();

            // Assert
            Assert.Equal(0, score.Mode);
            Assert.Equal((ulong)0, score.Rank);
            Assert.Null(score.PlayerName);
            Assert.Equal(0.0, score.ScoreResult);
            Assert.Null(score.Country);
            Assert.Null(score.PhotoURL);
            Assert.False(score.IsCurrentUser);
        }

        [Fact]
        public void Score_CanRepresentCurrentUser()
        {
            // Arrange & Act
            var score = new Score
            {
                Mode = 2,
                Rank = 1,
                PlayerName = "CurrentUser",
                ScoreResult = 99999.99,
                Country = "PT",
                PhotoURL = "https://example.com/me.jpg",
                IsCurrentUser = true
            };

            // Assert
            Assert.True(score.IsCurrentUser);
            Assert.Equal("CurrentUser", score.PlayerName);
        }
    }
}
