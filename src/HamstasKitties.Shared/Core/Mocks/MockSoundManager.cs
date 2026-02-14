using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core.Mocks;

public class MockSoundManager : ISoundManager
{
    public bool IsMusicEnabled { get; set; } = true;
    public bool IsSoundFXEnabled { get; set; } = true;
    public float MasterVolume { get; set; } = 1.0f;

    public int PlaySoundCallCount { get; private set; }
    public int PlaySongCallCount { get; private set; }

    public bool Initialize() => true;
    public bool Finalize() => true;

    public void PlaySound(object sound) => PlaySoundCallCount++;
    public void PlaySong(object song, bool loop) => PlaySongCallCount++;
    public void StopCurrentSong() { }
    public void PauseCurrentSong() { }
    public void ResumeCurrentSong() { }
}
