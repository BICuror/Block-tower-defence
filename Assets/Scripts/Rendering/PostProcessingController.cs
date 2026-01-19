using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine;

public sealed class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume _volumeControllerPrefab;
    
    private Volume _currentVolume;

    public void SetProfile(VolumeProfile profile)
    {
        _currentVolume = Instantiate(_volumeControllerPrefab, transform);
        
        _currentVolume.profile = profile;
        _currentVolume.weight = 1f;
    }

    public async UniTask ChangeCustomVolume(VolumeProfile profile, float changeDuration)
    {
        Volume newVolume = Instantiate(_volumeControllerPrefab, transform);
        
        newVolume.profile = profile;
        
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
}