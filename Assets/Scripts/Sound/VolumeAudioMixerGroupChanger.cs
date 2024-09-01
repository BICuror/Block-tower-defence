using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public sealed class VolumeAudioMixerGroupChanger : MonoBehaviour
{
    public UnityEvent<float> MasterVolumeLoaded;
    public UnityEvent<float> EffectsVolumeLoaded;
    public UnityEvent<float> MusicVolumeLoaded;

    [SerializeField] private float _minVolume, _maxVolume;

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private float _defaultVolume;

    private void Start() => LoadVolumes();

    #region Loading
    public void LoadVolumes()
    {
        LoadMasterVolume();

        LoadEffectsVolume();

        LoadMusicVolume();
    }

    private void LoadMasterVolume() 
    {
        float masterVolume = 1f;

        if (PlayerPrefs.HasKey("MasterVolume")) masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        MasterVolumeLoaded.Invoke(masterVolume);
    }
    private void LoadEffectsVolume()
    {   
        float effectsVolume = _defaultVolume;

        if (PlayerPrefs.HasKey("EffectsVolume")) effectsVolume = PlayerPrefs.GetFloat("EffectsVolume");   
        EffectsVolumeLoaded.Invoke(effectsVolume);
    }

    private void LoadMusicVolume() 
    {
        float musicVolume = _defaultVolume;
        if (PlayerPrefs.HasKey("MusicVolume")) musicVolume = PlayerPrefs.GetFloat("MusicVolume");   
        MusicVolumeLoaded.Invoke(musicVolume);
    }
    #endregion

    #region Setting
    public void SetMasterVolume(float value) => SetVolume("MasterVolume", value);
    public void SetMusicVolume(float value) => SetVolume("MusicVolume", value);
    public void SetEffectsVolume(float value) => SetVolume("EffectsVolume", value);
    
    private void SetVolume(string parameter, float value) 
    {
        SetMixerVolume(parameter, value);

        PlayerPrefs.SetFloat(parameter, value);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        float volume = Mathf.Lerp(_minVolume, _maxVolume, value);

        mixer.SetFloat(parameter, volume);
    }
    #endregion
}
