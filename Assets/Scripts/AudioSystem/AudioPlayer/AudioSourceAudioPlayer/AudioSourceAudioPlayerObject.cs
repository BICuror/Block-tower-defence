using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CuroAudio
{
    public sealed class AudioSourceAudioPlayerObject : MonoBehaviour
    {
        [Header("AudioSources")] 
        [SerializeField] private AudioSource _sFXAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _ambienceAudioSource;
        
        [Header("SFXAudioSourcePool")]
        [SerializeField] private AudioSource _sfxAudioSourcePrefab;
        private AudioSourcePool _audioSourcePool;
        
        [Header("AudioMixerGroups")] 
        [SerializeField] private AudioMixer _mainAudioMixer;
        private bool _initialized;
            
        public AudioSourcePool SFXAudioSourcePool => _audioSourcePool;
        public AudioSource SFXAudioSource => _sFXAudioSource;
        public AudioSource MusicAudioSource => _musicAudioSource;
        public AudioSource AmbienceAudioSource => _ambienceAudioSource;
        public AudioMixer MainAudioMixer => _mainAudioMixer;
        
        public bool Initialized => _initialized;
        
        private void Start() => _initialized = true;

        public void InitializeSFXAudioSourcePool()
        {
            _audioSourcePool = new(_sfxAudioSourcePrefab, transform);
        }

        public class AudioSourcePool
        {
            private readonly AudioSource _sourcePrefab;
            private readonly Transform _parentTransform;

            private readonly List<AudioSource> _unusedSources = new();
            
            public AudioSourcePool(AudioSource sourcePrefab, Transform parent)
            {
                _sourcePrefab = sourcePrefab;
                _parentTransform = parent;
            }

            public AudioSource GetSource()
            {
                if (_unusedSources.Count > 0)
                {
                    AudioSource source = _unusedSources[^1];
                    _unusedSources.RemoveAt(_unusedSources.Count - 1);
                    source.gameObject.SetActive(true);
                    return source;
                }
                
                return Instantiate(_sourcePrefab, _parentTransform);
            }

            public void ReturnSource(AudioSource source)
            {
                source.gameObject.SetActive(false);
                _unusedSources.Add(source);
            }
        }
    }
}