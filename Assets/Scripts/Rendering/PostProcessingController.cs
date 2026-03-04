using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using CuroSettings;
using DG.Tweening;
using UnityEngine;

public sealed class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume _volumeControllerPrefab;
    [SerializeField] private Volume _defaultVolume;
    
    private BoolSetting _postProcessingSetting;
    private Volume _currentVolume;

    private void Awake()
    {
        _postProcessingSetting = SettingsContainer.GetSetting<BoolSetting>(SettingsEnum.PostProcessingEnabled);
        _postProcessingSetting.ValueChanged += UpdatePostProcessingState;
        UpdatePostProcessingState();
    }

    private void UpdatePostProcessingState()
    {
        if (_currentVolume) _currentVolume.enabled = _postProcessingSetting.Value;
        _defaultVolume.enabled = _postProcessingSetting.Value;
    }
    
    public void SetProfile(VolumeProfile profile)
    {
        _currentVolume = Instantiate(_volumeControllerPrefab, transform);
        
        _currentVolume.profile = profile;
        _currentVolume.enabled = _postProcessingSetting.Value;
        _currentVolume.weight = 1f;
    }

    public async UniTask ChangeCustomVolume(VolumeProfile profile, float changeDuration)
    {
        Volume newVolume = Instantiate(_volumeControllerPrefab, transform);
        
        newVolume.profile = profile;
        newVolume.enabled = _postProcessingSetting.Value;
        
        await UniTask.WaitUntil(() => newVolume.HasInstantiatedProfile());
        
        await DOVirtual.Float(0f, 1f, changeDuration, UpdateProfilesWeight).SetEase(Ease.Linear).AsyncWaitForCompletion();
        
        Destroy(_currentVolume.gameObject);
        
        _currentVolume = newVolume;
        
        return;

        void UpdateProfilesWeight(float progress)
        {
            newVolume.weight = progress;
            _currentVolume.weight = 1f - progress;
        }
    }

    private void OnDestroy()
    {
        _postProcessingSetting.ValueChanged -= UpdatePostProcessingState;
    }
}