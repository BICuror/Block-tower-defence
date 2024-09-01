using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public sealed class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    [Header("PitchSettings")]
    [SerializeField] private SoundPicthChangeStrategy _pitchChangeStrategyType;
    [Range(0f, 1f)] [SerializeField] private float _pitchChangeStrength;

    private float _defaultPitch;

    public delegate float PicthChangeStrategy();
    private PicthChangeStrategy _pitchChangeStrategy;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _defaultPitch = _audioSource.pitch;

        switch(_pitchChangeStrategyType)
        {
            case SoundPicthChangeStrategy.DefaultIsMiddle: _pitchChangeStrategy = ChangePitchWithDefaultBeingMiddle; break;
            case SoundPicthChangeStrategy.DefaultIsHighest: _pitchChangeStrategy = ChangePitchWithDefaultBeingHighest; break;
            case SoundPicthChangeStrategy.DefaultIsLowest: _pitchChangeStrategy = ChangePitchWithDefaultBeingLowest; break;
        }
    }   

    private float ChangePitchWithDefaultBeingMiddle() => _audioSource.pitch = _defaultPitch + Random.Range(-_pitchChangeStrength, _pitchChangeStrength);
    private float ChangePitchWithDefaultBeingHighest() => _audioSource.pitch = _defaultPitch + Random.Range(-_pitchChangeStrength, 0f);
    private float ChangePitchWithDefaultBeingLowest() => _audioSource.pitch = _defaultPitch + Random.Range(0f, _pitchChangeStrength);

    public void Play()
    {
        _pitchChangeStrategy();

        _audioSource.Play();
    }

    public void PlayOneShot()
    {
        _pitchChangeStrategy();
        
        _audioSource.PlayOneShot(_audioSource.clip);
    }

    private enum SoundPicthChangeStrategy
    {
        DefaultIsMiddle,
        DefaultIsHighest,
        DefaultIsLowest
    }
}
