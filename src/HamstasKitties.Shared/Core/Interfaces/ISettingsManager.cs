namespace HamstasKitties.Core.Interfaces;

public interface ISettingsManager : IManager
{
    void SaveSetting(string id, object value);
    T LoadSetting<T>(string id);
    bool ContainsSetting(string id);
    void RemoveSetting(string id);
}
