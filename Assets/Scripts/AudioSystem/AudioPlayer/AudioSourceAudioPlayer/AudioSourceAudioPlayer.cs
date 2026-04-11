using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Linq;

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

        void IAudioPlayer.PlayMusic(MusicReference reference, AudioLayer layer, bool removeAllOther, bool awaitStopToStart)
        {
            _musicModule.PlayAudio(reference, layer, removeAllOther, awaitStopToStart).Forget();
        }

        void IAudioPlayer.PlayAmbience(AmbienceReference reference, AudioLayer layer, bool removeAllOther, bool awaitStopToStart)
        {
            _ambienceModule.PlayAudio(reference, layer, removeAllOther, awaitStopToStart).Forget();
        }

        bool IAudioPlayer.TryEnqueueMusic(MusicReference reference, AudioLayer layer, bool removeAllOther, bool awaitStopToStart)
        {
            return _musicModule.TryEnqueueAudio(reference, layer, removeAllOther, awaitStopToStart);
        }

        bool IAudioPlayer.TryEnqueueAmbience(AmbienceReference reference, AudioLayer layer, bool removeAllOther, bool awaitStopToStart)
        {
            return _ambienceModule.TryEnqueueAudio(reference, layer, removeAllOther, awaitStopToStart);
        }

        public void StopMusic(AudioLayer layer, bool tryActivateLowerPriorityAudio = true, bool awaitStopToStart = true)
        {
            _musicModule.StopAudio(layer, tryActivateLowerPriorityAudio, awaitStopToStart).Forget();
        }

        void IAudioPlayer.StopAmbience(AudioLayer layer, bool tryActivateLowerPriorityAudio, bool awaitStopToStart)
        {
            _ambienceModule.StopAudio(layer, tryActivateLowerPriorityAudio, awaitStopToStart).Forget();
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
                    break;
                } 
                case AudioChannelType.Music:
                {
                    _audioSources.MainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
                    break;
                } 
                case AudioChannelType.Ambience:
                {
                    _audioSources.MainAudioMixer.SetFloat("AmbienceVolume", Mathf.Log10(value) * 20);
                    break;
                } 
            }
        }
        
        #endregion

        private abstract class TransitionAudioModule<T> where T : AudioReferenceWithTransition
        {
            private AudioChannelLayerContainer<T> _layerContainer = new();

            public bool TryEnqueueAudio(T reference, AudioLayer layer, bool removeAllOther, bool awaitStopToStart)
            {
                if (_layerContainer.HasLayer(layer)) return false;
                
                if (_layerContainer.GetHighestPriorityLayer() == layer)
                {
                    PlayAudio(reference, layer, removeAllOther, awaitStopToStart).Forget();
                }
                else
                {
                    _layerContainer.SetActiveReferenceToLayer(reference, layer);
                }
                
                return true;
            }
            
            public async UniTask StopAudio(AudioLayer layer, bool tryActivateLowerPriorityAudio, bool awaitStopToStart)
            {
                if (awaitStopToStart) await StopLayer(layer);
                else StopLayer(layer).Forget();
                
                if (!tryActivateLowerPriorityAudio || _layerContainer.IsEmpty) return;

                AudioLayer highestPriorityLayer = _layerContainer.GetHighestPriorityLayer();

                await PlayLayer(highestPriorityLayer);
            }
            
            public async UniTask PlayAudio(T reference, AudioLayer layer, bool removeAllOther, bool awaitStopToStart)
            {
                bool layerContainsReference = LayerContainsReference(layer, reference);
                
                if (removeAllOther)
                {
                    List<AudioLayer> excludedLayers = new();
                    
                    if (layerContainsReference) excludedLayers.Add(layer);
                    
                    if (awaitStopToStart) await StopAllLayers(excludedLayers);
                    else StopAllLayers(excludedLayers).Forget();
                }
                else if (!layerContainsReference)
                { 
                    if (awaitStopToStart) await StopLayer(layer);
                    else StopLayer(layer).Forget();
                }
                
                _layerContainer.SetActiveReferenceToLayer(reference, layer);
                
                await PlayLayer(layer);
            }
            
            private bool LayerContainsReference(AudioLayer layer, T reference)
            {
                return _layerContainer.HasLayer(layer) && _layerContainer.GetAudioReference(layer) == reference;
            }
            
            private async UniTask StopAllLayers(List<AudioLayer> excludedLayers)
            {
                List<AudioLayer> audioLayersToStop = _layerContainer.GetAllPresentAudioLayers().Except(excludedLayers).ToList();

                float maxAudioLength = 0f;

                audioLayersToStop.ForEach(layer =>
                {
                    maxAudioLength = Mathf.Max(maxAudioLength, _layerContainer.GetAudioReference(layer).TransitionDuration);
                });
                
                audioLayersToStop.ForEach(layer => StopLayer(layer).Forget());

                await UniTask.WaitForSeconds(maxAudioLength, ignoreTimeScale: true);
            }
            
            private async UniTask StopLayer(AudioLayer layer)
            {
                if (!_layerContainer.HasLayer(layer)) return;
                
                T audioReference = _layerContainer.GetAudioReference(layer);
                AudioSource source = GetAudioSource(layer);

                ReplaceAudioSource(layer);
                
                await AudioUtility.DoVirtual(1f, 0f, audioReference.TransitionDuration, value => SetSourceVolume(source, value * audioReference.VolumeModifier));
                source.Stop();
                AudioAssetProvider.UnloadAudioAsset(audioReference);
                
                _layerContainer.RemoveLayer(layer);
            }
            
            private async UniTask PlayLayer(AudioLayer layer)
            {
                T audioReference = _layerContainer.GetAudioReference(layer);
                AudioSource source = GetAudioSource(layer);
                
                source.clip = await AudioAssetProvider.LoadAudioClipsFromReference(audioReference);
                source.Play();
                
                await AudioUtility.DoVirtual(0f, 1f, audioReference.TransitionDuration, value => SetSourceVolume(source, value * audioReference.VolumeModifier));
            }
            
            private void SetSourceVolume(AudioSource source, float volume)
            {
                source.volume = volume;
            }
            
            protected abstract AudioSource GetAudioSource(AudioLayer layer);
            protected abstract void ReplaceAudioSource(AudioLayer layer);
        }
        
        private sealed class MusicModule : TransitionAudioModule<MusicReference>
        {
            private AudioSourceAudioPlayerObject _audioSources;

            public MusicModule(AudioSourceAudioPlayerObject audioSources)
            {
                _audioSources = audioSources;
            }

            protected override AudioSource GetAudioSource(AudioLayer layer) => _audioSources.GetMusicAudioSource(layer);
            protected override void ReplaceAudioSource(AudioLayer layer) => _audioSources.ReplaceMusicAudioSource(layer).Forget();
        }

        private sealed class AmbienceModule : TransitionAudioModule<AmbienceReference>
        {
            private AudioSourceAudioPlayerObject _audioSources;

            public AmbienceModule(AudioSourceAudioPlayerObject audioSources)
            {
                _audioSources = audioSources;
            }

            protected override AudioSource GetAudioSource(AudioLayer layer) => _audioSources.GetAmbienceAudioSource(layer);
            protected override void ReplaceAudioSource(AudioLayer layer) => _audioSources.ReplaceAmbienceAudioSource(layer).Forget();
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
                AudioSourcePoolObject sourcePoolObject = _audioSources.SFXAudioSourcePool.GetSource();
                sourcePoolObject.transform.position = position;
                
                ApplyReferenceSettingsToSource(sfxReference, sourcePoolObject.Source);

                await sourcePoolObject.PlayAudioClip(clip);
                
                _audioSources.SFXAudioSourcePool.ReturnSource(sourcePoolObject);
            }
            
            public async UniTask PlaySFX(SFXReference sfxReference, CancellationToken cancellationToken)
            {
                AudioClip clip = await AudioAssetProvider.LoadAudioClipsFromReference(sfxReference);
                AudioSourcePoolObject sourcePoolObject = _audioSources.SFXAudioSourcePool.GetSource();
                
                ApplyReferenceSettingsToSource(sfxReference, sourcePoolObject.Source);

                await sourcePoolObject.PlayAudioClip(clip, cancellationToken);
                
                _audioSources.SFXAudioSourcePool.ReturnSource(sourcePoolObject);
            }

            private void ApplyReferenceSettingsToSource(SFXReference sfxReference, AudioSource audioSource)
            {
                audioSource.volume = sfxReference.VolumeModifier;
                audioSource.pitch = sfxReference.Pitch;
                
                if (sfxReference.UseRandomPitch)
                {
                    audioSource.pitch += Random.Range(1f - sfxReference.PitchMagnitude, 1f + sfxReference.PitchMagnitude);
                }
            }
        }
    }
}