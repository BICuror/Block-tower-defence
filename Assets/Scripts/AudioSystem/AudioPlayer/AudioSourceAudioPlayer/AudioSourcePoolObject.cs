using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]

public sealed class AudioSourcePoolObject : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public Action ClipStarted;
    public Action ClipEnded;
    public Action<AudioSourcePoolObject> AudioSourceReturned;
    
    public AudioSource Source => _audioSource;
    
    public async UniTask PlayAudioClip(AudioClip clip, CancellationToken cancellationToken = default)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
        
        ClipStarted?.Invoke();

        try
        {
            await UniTask.WaitWhile(() => _audioSource.isPlaying, cancellationToken: cancellationToken);
        }
        catch
        {
            _audioSource.Stop();
        }
        
        _audioSource.clip = null;
        ClipEnded?.Invoke();
        AudioSourceReturned?.Invoke(this);
    }
}