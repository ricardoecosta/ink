using FluentAssertions;
using HamstasKitties.Core.Mocks;
using Xunit;

namespace HamstasKitties.Shared.Tests.Services;

public class MockSettingsManagerTests
{
    [Fact]
    public void Initialize_ReturnsTrue()
    {
        // Arrange
        var manager = new MockSettingsManager();

        // Act
        var result = manager.Initialize();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Finalize_ReturnsTrue()
    {
        // Arrange
        var manager = new MockSettingsManager();

        // Act
        var result = manager.Finalize();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SaveSetting_ThenLoadSetting_ReturnsSavedValue()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "test_key";
        const string value = "test_value";

        // Act
        manager.SaveSetting(key, value);
        var result = manager.LoadSetting<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void SaveSetting_WithInteger_ThenLoadSetting_ReturnsSavedValue()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "int_key";
        const int value = 42;

        // Act
        manager.SaveSetting(key, value);
        var result = manager.LoadSetting<int>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void SaveSetting_WithBoolean_ThenLoadSetting_ReturnsSavedValue()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "bool_key";
        const bool value = true;

        // Act
        manager.SaveSetting(key, value);
        var result = manager.LoadSetting<bool>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void SaveSetting_WithFloat_ThenLoadSetting_ReturnsSavedValue()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "float_key";
        const float value = 3.14f;

        // Act
        manager.SaveSetting(key, value);
        var result = manager.LoadSetting<float>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void ContainsSetting_WhenSettingExists_ReturnsTrue()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "existing_key";

        // Act
        manager.SaveSetting(key, "value");
        var result = manager.ContainsSetting(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ContainsSetting_WhenSettingDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "nonexistent_key";

        // Act
        var result = manager.ContainsSetting(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RemoveSetting_RemovesExistingSetting()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "removable_key";

        // Act
        manager.SaveSetting(key, "value");
        manager.RemoveSetting(key);
        var result = manager.ContainsSetting(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RemoveSetting_WhenSettingDoesNotExist_DoesNotThrow()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "nonexistent_key";

        // Act & Assert
        manager.Invoking(m => m.RemoveSetting(key)).Should().NotThrow();
    }

    [Fact]
    public void SaveSetting_WithSameKey_OverwritesPreviousValue()
    {
        // Arrange
        var manager = new MockSettingsManager();
        const string key = "overwrite_key";

        // Act
        manager.SaveSetting(key, "original");
        manager.SaveSetting(key, "updated");
        var result = manager.LoadSetting<string>(key);

        // Assert
        result.Should().Be("updated");
    }

    [Fact]
    public void MultipleSettings_CanBeStoredAndRetrieved()
    {
        // Arrange
        var manager = new MockSettingsManager();

        // Act
        manager.SaveSetting("key1", "value1");
        manager.SaveSetting("key2", 42);
        manager.SaveSetting("key3", true);

        // Assert
        manager.LoadSetting<string>("key1").Should().Be("value1");
        manager.LoadSetting<int>("key2").Should().Be(42);
        manager.LoadSetting<bool>("key3").Should().BeTrue();
    }
}
