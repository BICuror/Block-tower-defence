using CuroSettings;
using UnityEngine;

public sealed class TimeController : MonoBehaviour
{
    private const float DEFAULT_TIME_SCALE = 1f;

    private bool _speedUpTime;
        
    private void Awake() => SetDefaultTimeScale();
    
    public void Resume()
    {
        if (_speedUpTime) SetSpeedUpTimeScale();
        else SetDefaultTimeScale();
    }
    
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void ToggleTimeScale()
    {
        if (!_speedUpTime) SetSpeedUpTimeScale();
        else SetDefaultTimeScale();
    }
    
    private void SetDefaultTimeScale()
    {
        Time.timeScale = DEFAULT_TIME_SCALE;
        _speedUpTime = false;
    }

    private void SetSpeedUpTimeScale()
    {
        Time.timeScale = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.SpeedUpTimeScale).Value;
        _speedUpTime = true;
    }
        
    private void OnDestroy() => SetDefaultTimeScale();
}