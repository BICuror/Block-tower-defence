using UnityEngine;

[RequireComponent(typeof(AudioSourcePoolObject))]

public sealed class FocusAudioSource : FocusElement
{
    [SerializeField] private AudioSourcePoolObject _audioSourcePoolObject;
    
    [Header("Stereo")]
    [SerializeField] private float _maxStereoChange = 0.5f;
    private float _initialVolume;
    
    private void Awake()
    {
        _audioSourcePoolObject.ClipStarted += CaptureClipVolumeAndStartUpdatingFocusValue;
        _audioSourcePoolObject.ClipEnded += StopUpdatingFocusValue;
    }

    private void CaptureClipVolumeAndStartUpdatingFocusValue()
    {
        _initialVolume = _audioSourcePoolObject.Source.volume;
        
        StartUpdatingFocusValue();
    }
    
    protected override void OnFocusChanged()
    {
        _audioSourcePoolObject.Source.volume = _initialVolume * Focus;
    }

    protected override void OnViewportPositionChanged(Vector2 position)
    {
        _audioSourcePoolObject.Source.panStereo = (position.x - 0.5f) * _maxStereoChange;
    }
}