using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

namespace CuroAudio
{
    public static class AudioAssetProvider
    {
        private static readonly Dictionary<AudioAssetLifetimeDuration, int> _lifetimeDurations = new Dictionary<AudioAssetLifetimeDuration, int>
        {
            {AudioAssetLifetimeDuration.Short, 15},
            {AudioAssetLifetimeDuration.Medium, 30},
            {AudioAssetLifetimeDuration.Long, 90}
        };
        
        private static readonly Dictionary<AudioReference, AudioAssetUnloadTimer> _unloadTimers = new();

        public static async UniTask<AudioClip> LoadAudioClipsFromReference(AudioReference audioReference)
        {
            if (_unloadTimers.ContainsKey(audioReference))
            {
                _unloadTimers[audioReference].ResetTimerDuration();
            }
            else
            {
                await CreateNewUnloadTimer(audioReference);
            }

            await UniTask.WaitUntil(() => _unloadTimers[audioReference].AssetsAreLoaded);
            
            return _unloadTimers[audioReference].RandomClip;
        }

        public static void UnloadAudioAsset(AudioReference audioReference)
        {
            _unloadTimers[audioReference].Unload();
        }

        private static async UniTask<AudioAssetUnloadTimer> CreateNewUnloadTimer(AudioReference audioReference)
        {
            AudioAssetUnloadTimer newUnloadTimer = new(audioReference);
            _unloadTimers[audioReference] = newUnloadTimer;
            newUnloadTimer.Unloaded += RemoveAudioReferenceUnloadTimer;

            SFXReference sfxReference = audioReference as SFXReference;

            if (sfxReference != null && sfxReference.UseRandomSFX)
            {
                await newUnloadTimer.LoadAssets(sfxReference.RandomSFXReferences);
            }
            else
            {
                await newUnloadTimer.LoadAssets(audioReference.AudioFileReference);
            }
            
            newUnloadTimer.StartTimer().Forget();
            
            return newUnloadTimer;
        }

        private static void RemoveAudioReferenceUnloadTimer(AudioAssetUnloadTimer unloadTimer)
        {
            unloadTimer.Unloaded -= RemoveAudioReferenceUnloadTimer;
            _unloadTimers.Remove(unloadTimer.AudioReference);
        }
        
        private sealed class AudioAssetUnloadTimer
        {
            private readonly AudioReference _associatedAudioReference;
            private readonly CancellationTokenSource _unloadCancellationTokenSource = new();
            private readonly List<AudioClip> _clips = new();
            private bool _assetsAreLoaded;
            private int _leftDuration;
            
            public AudioReference AudioReference => _associatedAudioReference;
            public AudioClip RandomClip => _clips[Random.Range(0, _clips.Count)];
            public bool AssetsAreLoaded => _assetsAreLoaded;
            
            public event Action<AudioAssetUnloadTimer> Unloaded; 
            
            public AudioAssetUnloadTimer(AudioReference audioReference)
            {
                _associatedAudioReference = audioReference;
            }

            public UniTask LoadAssets(AssetReference reference) => LoadAssets(new List<AssetReference> { reference });
            public async UniTask LoadAssets(List<AssetReference> references)
            {
                List<UniTask> loadTasks = new();
                references.ForEach(reference =>
                {
                    loadTasks.Add(AddLoadedAudioClip(reference));
                });

                await UniTask.WhenAll(loadTasks);
                _assetsAreLoaded = true;
                
                return;
                
                async UniTask AddLoadedAudioClip(AssetReference reference) 
                {
                    _clips.Add(await Addressables.LoadAssetAsync<AudioClip>(reference)); 
                }
            }
            
            public void ResetTimerDuration()
            {
                if (!_associatedAudioReference.HasLifetimeDuration) return;
                
                _leftDuration = _lifetimeDurations[_associatedAudioReference.LifetimeDuration];
            }

            public async UniTask StartTimer()
            {
                if (!_associatedAudioReference.HasLifetimeDuration) return;
                
                ResetTimerDuration();
                
                while (_leftDuration > 0)
                {
                    try
                    {
                        await UniTask.WaitForSeconds(1f, true, cancellationToken: _unloadCancellationTokenSource.Token);
                    }
                    catch { return; }

                    _leftDuration--;
                }
                
                Unload();
            }

            public void Unload()
            {
                Unloaded?.Invoke(this);
                _unloadCancellationTokenSource.Cancel();
                _clips.ForEach(clip =>
                {
                    //Addressables.Release(clip);
                    Resources.UnloadAsset(clip);
                });
            }
        }
    }
}