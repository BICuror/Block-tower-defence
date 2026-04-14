using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace CuroAudio
{
    public static class AudioSystem
    {
        private static readonly Dictionary<AudioEnum, AudioReference> _cachedAudioReferences = new();
        private static AudioSystemConfig _audioSystemConfig;
        private static IAudioPlayer _audioPlayer;
        
        private static readonly Dictionary<AudioChannelType, bool> _audioChannelsMuteState = new();
        private static bool _masterMuteState;
        
        public static void Initialize(IAudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _audioSystemConfig = AudioUtility.GetAudioSystemConfig();
            
            CacheAudioReferences();
        }

        public static void PlaySFX(SFXReference reference)
        {
            if (ChannelIsMuted(AudioChannelType.SFX)) return;
            
            _audioPlayer.PlaySFX(reference);
        }
        
        public static void PlaySFX(SFXReference reference, Vector3 position)
        {
            if (ChannelIsMuted(AudioChannelType.SFX)) return;
            
            _audioPlayer.PlaySFX(reference, position);
        }
        
        public static void PlaySFX(SFXReference reference, CancellationToken token)
        {
            if (ChannelIsMuted(AudioChannelType.SFX)) return;
            
            _audioPlayer.PlaySFX(reference, token);
        }

        public static UniTask<AudioSource> PlayLoopSFX(SFXReference reference)
        {
            return _audioPlayer.PlayLoopSFX(reference);
        }

        public static void PlayMusic(MusicReference reference, AudioLayer layer, bool removeAllOther = false, bool awaitStopToStart = false)
        {
            _audioPlayer.PlayMusic(reference, layer, removeAllOther, awaitStopToStart);
        }

        public static void PlayAmbience(AmbienceReference reference, AudioLayer layer, bool removeAllOther = false, bool awaitStopToStart = false)
        {
            _audioPlayer.PlayAmbience(reference, layer, removeAllOther, awaitStopToStart);
        }

        public static void StopMusic(AudioLayer layer, bool tryActivateLowerPriorityAudio = true, bool awaitStopToStart = false)
        {
            _audioPlayer.StopMusic(layer, tryActivateLowerPriorityAudio, awaitStopToStart);
        }

        public static void StopAmbience(AudioLayer layer, bool tryActivateLowerPriorityAudio = true, bool awaitStopToStart = false)
        {
            _audioPlayer.StopAmbience(layer, tryActivateLowerPriorityAudio, awaitStopToStart);
        }

        #region Volume

        public static void SetVolume(AudioChannelType audioChannelType, float volume)
        {
            _audioChannelsMuteState[audioChannelType] = volume == 0f;
            
            _audioPlayer.SetVolume(audioChannelType, volume);
        }

        public static void SetMasterVolume(float volume)
        {
            _masterMuteState = volume == 0f;
            
            _audioPlayer.SetMasterVolume(volume);
        }

        private static bool ChannelIsMuted(AudioChannelType audioChannelType) => _audioChannelsMuteState[audioChannelType] || _masterMuteState;
        
        #endregion    

        #region AudioEnumExtention

        public static void PlaySFX(AudioEnum audioEnum, CancellationToken token)
        {
            if (ChannelIsMuted(AudioChannelType.SFX)) return;
            
            AudioReference audioReference = GetAudioReference(audioEnum);
            
            if (audioReference is not SFXReference) throw new Exception($"Tried to play audioReference {audioReference.name} as SFXReference, while it is {audioReference.GetType()}");
            
            PlaySFX(audioReference as SFXReference, token);
        }
        
        public static void PlaySFX(AudioEnum audioEnum, Vector3 position)
        {
            if (ChannelIsMuted(AudioChannelType.SFX)) return;
            
            AudioReference audioReference = GetAudioReference(audioEnum);
            
            if (audioReference is not SFXReference) throw new Exception($"Tried to play audioReference {audioReference.name} as SFXReference, while it is {audioReference.GetType()}");
            
            PlaySFX(audioReference as SFXReference, position);
        }
        
        public static void PlaySFX(AudioEnum audioEnum)
        {
            if (ChannelIsMuted(AudioChannelType.SFX)) return;
            
            AudioReference audioReference = GetAudioReference(audioEnum);
            
            if (audioReference is not SFXReference) throw new Exception($"Tried to play audioReference {audioReference.name} as SFXReference, while it is {audioReference.GetType()}");
            
            PlaySFX(audioReference as SFXReference);
        }
        
        public static UniTask<AudioSource> PlayLoopSFX(AudioEnum audioEnum)
        {
            AudioReference audioReference = GetAudioReference(audioEnum);
            
            if (audioReference is not SFXReference) throw new Exception($"Tried to play audioReference {audioReference.name} as SFXReference, while it is {audioReference.GetType()}");
            
            return PlayLoopSFX(audioReference as SFXReference);
        }
    
        public static void PlayMusic(AudioEnum audioEnum, AudioLayer layer)
        {
            AudioReference audioReference = GetAudioReference(audioEnum);
            
            if (audioReference is not MusicReference) throw new Exception($"Tried to play audioReference {audioReference.name} as MusicReference, while it is {audioReference.GetType()}");
            
            PlayMusic(audioReference as MusicReference, layer);
        }
    
        public static void PlayAmbience(AudioEnum audioEnum, AudioLayer layer)
        {
            AudioReference audioReference = GetAudioReference(audioEnum);
            
            if (audioReference is not AmbienceReference) throw new Exception($"Tried to play audioReference {audioReference.name} as AmbienceReference, while it is {audioReference.GetType()}");
            
            PlayAmbience(audioReference as AmbienceReference, layer);
        }

        #endregion
        
        #region AudioReferenceFetch

        private static AudioReference GetAudioReference(AudioEnum audioEnum)
        {
            if (_cachedAudioReferences.TryGetValue(audioEnum, out AudioReference reference)) return reference;

            CacheAudioReferences();
            
            if (_cachedAudioReferences.TryGetValue(audioEnum, out reference)) return reference;
            
            Debug.LogError($"Audio reference with {audioEnum} was not found");
            
            throw new KeyNotFoundException($"Audio reference with {audioEnum} was not found");
        }

        private static void CacheAudioReferences()
        {
            _cachedAudioReferences.Clear();
            
            _audioSystemConfig.Sectors.ForEach(Sector =>
            {
                Sector.SubSectors.ForEach(SubSector =>
                {
                    SubSector.AudioReferenceEntries.ForEach(Entriy =>
                    {
                        _cachedAudioReferences[Entriy.EnumValue] = Entriy.AudioReference;
                    });
                });
            });
        }

        #endregion
    }
}