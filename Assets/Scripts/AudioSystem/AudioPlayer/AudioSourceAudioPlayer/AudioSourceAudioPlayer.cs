using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

using Object = UnityEngine.Object;

namespace CuroAudio
{
    public sealed class AudioSourceAudioPlayer : IAudioPlayer
    {
        private const float AUDIO_STEP_TRANSITION_DURATION = 1f;
        private AudioSourceAudioPlayerObject _audioSources;
        private AmbienceModule _ambienceModule;
        private MusicModule _musicModule;
        private SFXModule _sfxModule;
        
        private Dictionary<AudioChannelType, float> _channelVolumes = new();
        private float _masterVolume;

        void IAudioPlayer.Initialize()
        {
            _audioSources = Object.Instantiate(AudioUtility.GetAudioSystemConfig().AudioSourceAudioPlayerPrefab);
            Object.DontDestroyOnLoad(_audioSources);
            _audioSources.InitializeSFXAudioSourcePool();

            _ambienceModule = new(_audioSources);
            _musicModule = new(_audioSources);
            _sfxModule = new(_audioSources);

            AwaitToInitializeAudioMixerGroupsVolume().Forget();
        }
        
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Audio.AudioMixer.SetFloat.html#:~:text=To%20expose%20a%20parameter%2C%20go%20to%20the%20Audio,Audio%20Mixer%20group%20parameter%20to%20a%20new%20value
        // Unfortunately we cannot use RuntimeInitializeLoadType.AfterSceneLoad to SetFloat on AudioMixers since it will cause an unexpected behaviour
        // :o( sad af
        private async UniTask AwaitToInitializeAudioMixerGroupsVolume()
        {
            await UniTask.WaitUntil(() => _audioSources.Initialized);
            
            InitializeAudioMixerGroupsVolume();
        }
        
        private void InitializeAudioMixerGroupsVolume()
        {
            SetMasterVolume(_masterVolume);

            List<AudioChannelType> initializedChannelVolumeTypes = _channelVolumes.Keys.ToList();
            
            foreach (AudioChannelType channelVolumesKey in initializedChannelVolumeTypes)
            {
                SetVolume(channelVolumesKey, _channelVolumes[channelVolumesKey]);
            }
        }

        void IAudioPlayer.PlaySFX(SFXReference reference)
        {
            _sfxModule.PlaySFX(reference, _audioSources.SFXAudioSource).Forget();
        }

        void IAudioPlayer.PlaySFX(SFXReference reference, CancellationToken token)
        {
            _sfxModule.PlaySFX(reference, token).Forget();
        }

        void IAudioPlayer.PlaySFX(SFXReference reference, Vector3 position)
        {
            _sfxModule.PlaySFXAtPosition(reference, position).Forget();
        }

        void IAudioPlayer.PlayMusic(MusicReference reference, AudioLayer layer)
        {
            _musicModule.SetToLayerAndPlayHighestPriorityMusic(reference, layer).Forget();
        }
        
        void IAudioPlayer.PlayAmbience(AmbienceReference reference, AudioLayer layer)
        {
            _ambienceModule.SetToLayerAndPlayHighestPriorityAmbience(reference, layer).Forget();
        }

        void IAudioPlayer.StopMusic(AudioLayer layer)
        {
            _musicModule.StopAndPlayHighestPriorityMusic(layer).Forget();
        }

        void IAudioPlayer.StopAmbience(AudioLayer layer)
        {
            _ambienceModule.StopAndPlayHighestPriorityAmbience(layer).Forget();
        }

        #region VolumeControl

        public void SetMasterVolume(float value)
        {
            _masterVolume = value;
            
            // Unfortunately we cannot use RuntimeInitializeLoadType.AfterSceneLoad to SetFloat on AudioMixers since it will cause an unexpected behaviour
            if (!_audioSources.Initialized) return;
            
            if (value == 0f) value = 0.0001f;
            
            _audioSources.MainAudioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        }

        public void SetVolume(AudioChannelType channelType, float value)
        {
            _channelVolumes[channelType] = value;
            
            // Unfortunately we cannot use RuntimeInitializeLoadType.AfterSceneLoad to SetFloat on AudioMixers since it will cause an unexpected behaviour
            if (!_audioSources.Initialized) return;
            
            if (value == 0f) value = 0.0001f;
            
            switch (channelType)
            {
                case AudioChannelType.SFX:
                {
                    _audioSources.MainAudioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
                } break;
                case AudioChannelType.Music:
                {
                    _audioSources.MainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
                } break;
                case AudioChannelType.Ambience:
                {
                    _audioSources.MainAudioMixer.SetFloat("AmbienceVolume", Mathf.Log10(value) * 20);
                } break;
            }
        }
        
        #endregion
        
        private sealed class MusicModule
        {
            private AudioChannelLayerContainer<MusicReference> _layerContainer = new();
            private AudioSourceAudioPlayerObject _audioSources;
            private float _currentMusicVolumeModifier = 1f;

            public MusicModule(AudioSourceAudioPlayerObject audioSources)
            {
                _audioSources = audioSources;
            }        
            
            public async UniTask SetToLayerAndPlayHighestPriorityMusic(MusicReference reference, AudioLayer layer)
            {
                if (!_layerContainer.IsEmpty)
                {
                    MusicReference highestPriorityAudioReference = _layerContainer.GetHighestPriorityAudioReference();
                    if (reference == highestPriorityAudioReference) return;
                    
                    await StopMusic(layer);
                }
            
                _layerContainer.SetActiveReferenceToLayer(reference, layer);
                
                await PlayHighestPriorityMusic();
            }
            
            public async UniTask StopAndPlayHighestPriorityMusic(AudioLayer layer)
            {
                await StopMusic(layer);
                await PlayHighestPriorityMusic();
            }
            
            private async UniTask StopMusic(AudioLayer layer)
            {
                if (_layerContainer.IsEmpty) return;
                
                MusicReference activeMusicReference = _layerContainer.GetHighestPriorityAudioReference();
                _layerContainer.RemoveLayer(layer);
                await AudioUtility.DoVirtual(1f, 0f, activeMusicReference.TransitionDuration, SetMusicSourceVolume);
                AudioAssetProvider.UnloadAudioAsset(activeMusicReference);
            }
            
            private async UniTask PlayHighestPriorityMusic()
            {
                if (_layerContainer.IsEmpty) return;
                
                MusicReference musicReference = _layerContainer.GetHighestPriorityAudioReference();
                _currentMusicVolumeModifier = musicReference.VolumeModifier;
                _audioSources.MusicAudioSource.clip = await AudioAssetProvider.LoadAudioClipsFromReference(musicReference);
                _audioSources.MusicAudioSource.Play();
                await AudioUtility.DoVirtual(0f, 1f, musicReference.TransitionDuration, SetMusicSourceVolume);
            }
            
            private void SetMusicSourceVolume(float volume)
            {
                _audioSources.MusicAudioSource.volume = volume * _currentMusicVolumeModifier;
            }
        }

        private sealed class AmbienceModule
        {
            private AudioChannelLayerContainer<AmbienceReference> _layerContainer = new();
            private AudioSourceAudioPlayerObject _audioSources;
            private float _currentAmbienceVolumeModifier = 1f;
            
            public AmbienceModule(AudioSourceAudioPlayerObject audioSources)
            {
                _audioSources = audioSources;
            }

            public async UniTask SetToLayerAndPlayHighestPriorityAmbience(AmbienceReference reference, AudioLayer layer)
            {
                if (!_layerContainer.IsEmpty)
                {
                    AmbienceReference highestPriorityAudioReference = _layerContainer.GetHighestPriorityAudioReference();
                    if (reference == highestPriorityAudioReference) return;
                    
                    await StopAmbience(layer);
                }
            
                _layerContainer.SetActiveReferenceToLayer(reference, layer);
                
                await PlayHighestPriorityAmbience();
            }
            
            public async UniTask StopAndPlayHighestPriorityAmbience(AudioLayer layer)
            {
                await StopAmbience(layer);
                await PlayHighestPriorityAmbience();
            }
            
            private async UniTask StopAmbience(AudioLayer layer)
            {
                if (_layerContainer.IsEmpty) return;
                
                AmbienceReference activeAmbienceReference = _layerContainer.GetHighestPriorityAudioReference();
                _layerContainer.RemoveLayer(layer);
                await AudioUtility.DoVirtual(1f, 0f, activeAmbienceReference.TransitionDuration, SetAmbienceVolume);
                AudioAssetProvider.UnloadAudioAsset(activeAmbienceReference);
            }
            
            private async UniTask PlayHighestPriorityAmbience()
            {
                if (_layerContainer.IsEmpty) return;
                
                AmbienceReference ambienceReference = _layerContainer.GetHighestPriorityAudioReference();
                _currentAmbienceVolumeModifier = ambienceReference.VolumeModifier;
                _audioSources.AmbienceAudioSource.clip = await AudioAssetProvider.LoadAudioClipsFromReference(ambienceReference);
                _audioSources.AmbienceAudioSource.Play();
                await AudioUtility.DoVirtual(0f, 1f, ambienceReference.TransitionDuration, SetAmbienceVolume);
            }
            
            private void SetAmbienceVolume(float volume)
            {
                _audioSources.AmbienceAudioSource.volume = volume * _currentAmbienceVolumeModifier;
            }
        }

        private sealed class SFXModule
        {
            private AudioSourceAudioPlayerObject _audioSources;
            
            public SFXModule(AudioSourceAudioPlayerObject audioSources)
            {
                _audioSources = audioSources;
            }

            public async UniTask PlaySFX(SFXReference sfxReference, AudioSource audioSource)
            {
                AudioClip clip = await AudioAssetProvider.LoadAudioClipsFromReference(sfxReference);

                ApplyReferenceSettingsToSource(sfxReference, audioSource);
                
                audioSource.PlayOneShot(clip);
            }

            public async UniTask PlaySFXAtPosition(SFXReference sfxReference, Vector3 position)
            {
                AudioClip clip = await AudioAssetProvider.LoadAudioClipsFromReference(sfxReference);
                AudioSource source = _audioSources.SFXAudioSourcePool.GetSource();
                source.transform.position = position;
                
                ApplyReferenceSettingsToSource(sfxReference, source);

                source.clip = clip;
                source.Play();

                await UniTask.WaitForSeconds(clip.length);
                
                _audioSources.SFXAudioSourcePool.ReturnSource(source);
            }
            
            public async UniTask PlaySFX(SFXReference sfxReference, CancellationToken cancellationToken)
            {
                AudioClip clip = await AudioAssetProvider.LoadAudioClipsFromReference(sfxReference);
                AudioSource source = _audioSources.SFXAudioSourcePool.GetSource();
                
                ApplyReferenceSettingsToSource(sfxReference, source);

                source.clip = clip;
                source.Play();

                try
                {
                    await UniTask.WaitForSeconds(clip.length, cancellationToken: cancellationToken);
                }
                catch
                {
                    source.Stop();
                }
                
                _audioSources.SFXAudioSourcePool.ReturnSource(source);
            }

            private void ApplyReferenceSettingsToSource(SFXReference sfxReference, AudioSource audioSource)
            {
                audioSource.volume = sfxReference.VolumeModifier;
                
                if (sfxReference.UseRandomPitch)
                {
                    audioSource.pitch += Random.Range(1f - sfxReference.PitchMagnitude, 1f + sfxReference.PitchMagnitude);
                }
            }
        }
    }
}