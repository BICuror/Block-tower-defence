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
            {AudioAssetLifetimeDuration.Long, 120}
        };
        
        private static readonly Dictionary<AudioReference, AudioAssetUnloadTimer> _unloadTimers = new();

        public static async UniTask<AudioClip> LoadAudioClipsFromReference(AudioReference audioReference)
        {
            if (_unloadTimers.TryGetValue(audioReference, out AudioAssetUnloadTimer unloadTimer))
            {
                while (!unloadTimer.AssetsAreLoaded)
                {
                    await UniTask.WaitForFixedUpdate();
                }
                
                unloadTimer.ResetTimerDuration(); 
                return unloadTimer.RandomClip;
            }

            AudioAssetUnloadTimer newUnloadTimer = await CreateNewUnloadTimer(audioReference);

            return newUnloadTimer.RandomClip;
        }

        public static void UnloadAudioAsset(AudioReference audioReference)
        {
            _unloadTimers[audioReference].Unload();
        }

        private static async UniTask<AudioAssetUnloadTimer> CreateNewUnloadTimer(AudioReference audioReference)
        {
            AudioAssetUnloadTimer newUnloadTimer = new(audioReference);
            _unloadTimers[audioReference] = newUnloadTimer;
            newUnloadTimer.ReferenceAssetsUnloaded += RemoveAudioReferenceUnloadTimer;

            SFXReference sfxReference = audioReference as SFXReference;

            if (sfxReference != null && sfxReference.UseRandomSFX)
            {
                await newUnloadTimer.LoadAssets(sfxReference.RandomSFXReferences);
            }
            else
            {
                await newUnloadTimer.LoadAssets(audioReference.AudioFileReference);
            }
            
            if (audioReference.LifetimeDuration != AudioAssetLifetimeDuration.NoLifetime) newUnloadTimer.StartTimer().Forget();
            
            return newUnloadTimer;
        }

        private static void RemoveAudioReferenceUnloadTimer(AudioReference audioReference)
        {
            _unloadTimers.Remove(audioReference);
        }
        
        private sealed class AudioAssetUnloadTimer
        {
            private readonly AudioReference _associatedAudioReference;
            private readonly CancellationTokenSource _unloadCancellationTokenSource = new();
            private readonly List<AudioClip> _clips = new();
            private bool _assetsAreLoaded;
            private int _leftDuration;
            
            public AudioClip RandomClip => _clips[Random.Range(0, _clips.Count)];
            public bool AssetsAreLoaded => _assetsAreLoaded;
            
            public event Action<AudioReference> ReferenceAssetsUnloaded; 
            
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
                _leftDuration = _lifetimeDurations[_associatedAudioReference.LifetimeDuration];
            }

            public async UniTask StartTimer()
            {
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
                ReferenceAssetsUnloaded?.Invoke(_associatedAudioReference);
                _unloadCancellationTokenSource.Cancel();
                _clips.ForEach(Addressables.Release);
            }
        }
    }
}