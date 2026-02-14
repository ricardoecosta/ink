using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core.Mocks;

public class MockSettingsManager : ISettingsManager
{
    private readonly Dictionary<string, object> _settings = new();

    public bool Initialize() => true;
    public bool Finalize() => true;

    public void SaveSetting(string id, object value) => _settings[id] = value;
    public T LoadSetting<T>(string id) => (T)_settings[id];
    public bool ContainsSetting(string id) => _settings.ContainsKey(id);
    public void RemoveSetting(string id) => _settings.Remove(id);
}
