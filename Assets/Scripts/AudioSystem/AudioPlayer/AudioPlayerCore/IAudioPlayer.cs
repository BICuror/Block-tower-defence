using System.Threading;
using UnityEngine;

namespace CuroAudio
{
    public interface IAudioPlayer
    {
        public void Initialize();
        public void PlaySFX(SFXReference reference);
        public void PlaySFX(SFXReference reference, CancellationToken token);
        public void PlaySFX(SFXReference reference, Vector3 position);
        public void PlayMusic(MusicReference reference, AudioLayer layer);
        public void PlayAmbience(AmbienceReference reference, AudioLayer layer);
        
        public void StopMusic(AudioLayer layer);
        public void StopAmbience(AudioLayer layer);
        
        public void SetMasterVolume(float value);
        public void SetVolume(AudioChannelType channelType, float value);
    }
}