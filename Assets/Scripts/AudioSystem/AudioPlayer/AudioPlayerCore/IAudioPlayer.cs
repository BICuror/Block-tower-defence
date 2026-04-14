using Cysharp.Threading.Tasks;
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
        public UniTask<AudioSource> PlayLoopSFX(SFXReference reference);
        public void PlayMusic(MusicReference reference, AudioLayer layer, bool removeAllOther = true, bool awaitStopToStart = false);
        public void PlayAmbience(AmbienceReference reference, AudioLayer layer, bool removeAllOther = false, bool awaitStopToStart = false);
        
        public bool TryEnqueueMusic(MusicReference reference, AudioLayer layer, bool removeAllOther = true, bool awaitStopToStart = false);
        public bool TryEnqueueAmbience(AmbienceReference reference, AudioLayer layer, bool removeAllOther = false, bool awaitStopToStart = false);
        
        public void StopMusic(AudioLayer layer, bool tryActivateLowerPriorityAudio = true, bool awaitStopToStart = false);
        public void StopAmbience(AudioLayer layer, bool tryActivateLowerPriorityAudio = true, bool awaitStopToStart = false);
        
        public void SetMasterVolume(float value);
        public void SetVolume(AudioChannelType channelType, float value);
    }
}