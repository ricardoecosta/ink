namespace HamstasKitties.Core.Interfaces;

public interface ISoundManager : IManager
{
    bool IsMusicEnabled { get; set; }
    bool IsSoundFXEnabled { get; set; }
    float MasterVolume { get; set; }

    void PlaySound(object sound);
    void PlaySong(object song, bool loop);
    void StopCurrentSong();
    void PauseCurrentSong();
    void ResumeCurrentSong();
}
