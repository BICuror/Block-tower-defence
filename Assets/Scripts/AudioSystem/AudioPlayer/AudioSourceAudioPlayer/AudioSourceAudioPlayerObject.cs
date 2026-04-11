using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace CuroAudio
{
    public sealed class AudioSourceAudioPlayerObject : MonoBehaviour
    {
        [Header("AudioSources")] 
        [SerializeField] private AudioSource _sFXAudioSource;
        
        [Header("AudioSourcesPrefabs")]
        [SerializeField] private AudioSource _ambienceAudioSourcePrefab;
        [SerializeField] private AudioSource _musicAudioSourcePrefab;
        private Dictionary<AudioLayer, AudioSource> _ambienceAudioSources = new();
        private Dictionary<AudioLayer, AudioSource> _musicAudioSources = new();
        
        [Header("SFXAudioSourcePool")]
        [SerializeField] private AudioSourcePoolObject _sfxAudioSourcePrefab;
        private AudioSourcePool _audioSourcePool;
        
        [Header("AudioMixerGroups")] 
        [SerializeField] private AudioMixer _mainAudioMixer;
        private bool _initialized;
        
        public AudioSourcePool SFXAudioSourcePool => _audioSourcePool;
        public AudioSource SFXAudioSource => _sFXAudioSource;
        public AudioMixer MainAudioMixer => _mainAudioMixer;
        
        public bool Initialized => _initialized;
        
        private void Start() => _initialized = true;

        public void InitializeSFXAudioSourcePool()
        {
            _audioSourcePool = new(_sfxAudioSourcePrefab, transform);
        }

        public async UniTask ReplaceAmbienceAudioSource(AudioLayer layer) => await ReplaceAudioSource(layer, _ambienceAudioSources);
        public async UniTask ReplaceMusicAudioSource(AudioLayer layer) => await ReplaceAudioSource(layer, _musicAudioSources);
        
        public AudioSource GetAmbienceAudioSource(AudioLayer layer) => GetAudioSource(layer, _ambienceAudioSources, _ambienceAudioSourcePrefab);
        public AudioSource GetMusicAudioSource(AudioLayer layer) => GetAudioSource(layer, _musicAudioSources, _musicAudioSourcePrefab);
        
        private async UniTask ReplaceAudioSource(AudioLayer layer, Dictionary<AudioLayer, AudioSource> audioSources)
        {
            AudioSource audioSource = audioSources[layer];
            
            audioSources.Remove(layer);
            
            await UniTask.WaitWhile(() => audioSource.isPlaying);
            
            Destroy(audioSource.gameObject);
        }
        
        private AudioSource GetAudioSource(AudioLayer layer, Dictionary<AudioLayer, AudioSource> audioSources, AudioSource audioSourcePrefab)
        {
            if (!audioSources.ContainsKey(layer))
            {
                audioSources.Add(layer, Instantiate(audioSourcePrefab, transform));
            }

            return audioSources[layer];
        }

        public class AudioSourcePool
        {
            private readonly AudioSourcePoolObject _sourcePrefab;
            private readonly Transform _parentTransform;

            private readonly List<AudioSourcePoolObject> _unusedSources = new();
            
            public AudioSourcePool(AudioSourcePoolObject sourcePrefab, Transform parent)
            {
                _sourcePrefab = sourcePrefab;
                _parentTransform = parent;
            }

            public AudioSourcePoolObject GetSource()
            {
                if (_unusedSources.Count > 0)
                {
                    AudioSourcePoolObject source = _unusedSources[^1];
                    _unusedSources.RemoveAt(_unusedSources.Count - 1);
                    source.gameObject.SetActive(true);
                    return source;
                }
                
                return Instantiate(_sourcePrefab, _parentTransform);
            }

            public void ReturnSource(AudioSourcePoolObject source)
            {
                source.gameObject.SetActive(false);
                _unusedSources.Add(source);
            }
        }
    }
}