using System;
using Xunit;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;

namespace HamstasKitties.Tests.Social
{
    public class AchievementTests
    {
        [Fact]
        public void AchievementData_HasCorrectProperties()
        {
            // Arrange & Act
            var data = new AchievementData
            {
                Id = "test-achievement-1",
                Name = "Test Achievement",
                Description = "A test achievement",
                Reward = 100,
                Importance = 5,
                Completed = false
            };

            // Assert
            Assert.Equal("test-achievement-1", data.Id);
            Assert.Equal("Test Achievement", data.Name);
            Assert.Equal("A test achievement", data.Description);
            Assert.Equal(100, data.Reward);
            Assert.Equal(5, data.Importance);
            Assert.False(data.Completed);
        }

        [Fact]
        public void AchievementData_Equals_ReturnsTrueForSameData()
        {
            // Arrange
            var data1 = new AchievementData
            {
                Id = "test-achievement-1",
                Name = "Test Achievement",
                Description = "A test achievement",
                Reward = 100,
                Importance = 5,
                Completed = false
            };

            var data2 = new AchievementData
            {
                Id = "test-achievement-1",
                Name = "Test Achievement",
                Description = "A test achievement",
                Reward = 100,
                Importance = 5,
                Completed = false
            };

            // Act & Assert
            Assert.Equal(data1, data2);
            Assert.Equal(data1.GetHashCode(), data2.GetHashCode());
        }

        [Fact]
        public void AchievementData_Equals_ReturnsFalseForDifferentData()
        {
            // Arrange
            var data1 = new AchievementData
            {
                Id = "test-achievement-1",
                Name = "Test Achievement",
                Description = "A test achievement",
                Reward = 100,
                Importance = 5,
                Completed = false
            };

            var data2 = new AchievementData
            {
                Id = "test-achievement-2",
                Name = "Different Achievement",
                Description = "A different test achievement",
                Reward = 200,
                Importance = 3,
                Completed = true
            };

            // Act & Assert
            Assert.NotEqual(data1, data2);
        }

        [Fact]
        public void AchievementType_HasCorrectValues()
        {
            // Assert
            Assert.Equal(0, (int)Achievement.AchievementType.Normal);
            Assert.Equal(1, (int)Achievement.AchievementType.Runtime);
        }
    }

    // Test achievement class for testing abstract Achievement class
    public class TestAchievement : Achievement
    {
        public bool UpdateCalled { get; private set; }
        public TimeSpan LastTime { get; private set; }
        public Director LastDirector { get; private set; }

        public TestAchievement(string id, Achievement.AchievementType type, string name, string description, int reward, int importance)
            : base(id, type, name, description, reward, importance)
        {
        }

        public TestAchievement(AchievementData data, Achievement.AchievementType type)
            : base(data, type)
        {
        }

        public override void Update(TimeSpan time, Director director)
        {
            UpdateCalled = true;
            LastTime = time;
            LastDirector = director;
        }
    }

    public class AchievementConcreteTests
    {
        [Fact]
        public void Achievement_CreatesWithCorrectData()
        {
            // Arrange & Act
            var achievement = new TestAchievement(
                "test-1",
                Achievement.AchievementType.Normal,
                "Test Achievement",
                "Test Description",
                50,
                3
            );

            // Assert
            Assert.Equal("test-1", achievement.Data.Id);
            Assert.Equal("Test Achievement", achievement.Data.Name);
            Assert.Equal("Test Description", achievement.Data.Description);
            Assert.Equal(50, achievement.Data.Reward);
            Assert.Equal(3, achievement.Data.Importance);
            Assert.False(achievement.Data.Completed);
            Assert.Equal(Achievement.AchievementType.Normal, achievement.Type);
        }

        [Fact]
        public void Achievement_SetCompleted_MarksCompletedAndRaisesEvent()
        {
            // Arrange
            var achievement = new TestAchievement(
                "test-1",
                Achievement.AchievementType.Normal,
                "Test Achievement",
                "Test Description",
                50,
                3
            );

            Achievement eventAchievement = null;
            achievement.OnAchievementCompleted += (a) => eventAchievement = a;

            // Act
            achievement.SetCompleted();

            // Assert
            Assert.True(achievement.Data.Completed);
            Assert.Same(achievement, eventAchievement);
        }

        [Fact]
        public void Achievement_Equals_ReturnsTrueForSameData()
        {
            // Arrange
            var achievement1 = new TestAchievement(
                "test-1",
                Achievement.AchievementType.Normal,
                "Test Achievement",
                "Test Description",
                50,
                3
            );

            var achievement2 = new TestAchievement(
                "test-1",
                Achievement.AchievementType.Normal,
                "Test Achievement",
                "Test Description",
                50,
                3
            );

            // Act & Assert
            Assert.Equal(achievement1, achievement2);
        }
    }
}
