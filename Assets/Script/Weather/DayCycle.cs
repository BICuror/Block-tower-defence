using UnityEngine;
using UnityEngine.Events;

public sealed class DayCycle : MonoBehaviour
{
    [SerializeField] private int _secondsForDay;

    [SerializeField] private AnimationCurve _lightBrightnessCurve;

    [SerializeField] private AnimationCurve _lightAngleCurve;

    [SerializeField] private Material _skyboxMaterial;

    [SerializeField] private Light _globalLight;

    private int _currentTime;

    
    [Header("CycleSettings")]
    public UnityEvent MorningStarted;
    [Range(0f, 1f)] [SerializeField] private float _morningTime; 
    public UnityEvent DayStarted;
    [Range(0f, 1f)] [SerializeField] private float _dayTime;
    public UnityEvent EveningStarted;
    [Range(0f, 1f)] [SerializeField] private float _eveningTime;
    public UnityEvent NightStarted;
    [Range(0f, 1f)] [SerializeField] private float _nightTime;

    private DayPart _currentDayPart = DayPart.Day;

    private void Start()
    {
        _secondsForDay *= 50;
    
        _currentTime = Random.Range(0, _secondsForDay);
    }

    private void FixedUpdate()
    {
        _currentTime++;

        float evaluatedTime = (float)_currentTime / _secondsForDay;

        OffsetSkybox(evaluatedTime);

        ModifyLightning(evaluatedTime);

        UpdateDayTime(evaluatedTime);
    }

    private void UpdateDayTime(float value)
    {
        if (value <= _eveningTime)
        {
            if (_currentDayPart != DayPart.Evening)
            {
                _currentDayPart = DayPart.Evening;
                EveningStarted?.Invoke();
            }
        }
        else if (value <= _nightTime)
        {
            if (_currentDayPart != DayPart.Night)
            {
                _currentDayPart = DayPart.Night;
                NightStarted?.Invoke();
            }
        }
        else if (value <= _dayTime)
        {
            if (_currentDayPart != DayPart.Day)
            {
                _currentDayPart = DayPart.Day;
                DayStarted?.Invoke();   
            }     
        }
        else if (value <= _morningTime) 
        {
            if (_currentDayPart != DayPart.Morning)
            {
                _currentDayPart = DayPart.Morning;
                MorningStarted?.Invoke();
            }
        } 
        else if (value >= 1)
        {
            _currentTime = 0;
        }
    }

    private void OffsetSkybox(float value)
    {
        _skyboxMaterial.mainTextureOffset = new Vector2(value, 0f);
    }

    private void ModifyLightning(float value)
    {
        _globalLight.intensity = _lightBrightnessCurve.Evaluate(value);

        _globalLight.transform.rotation = Quaternion.Euler(_lightAngleCurve.Evaluate(value) * 180f, _globalLight.transform.rotation.y, _globalLight.transform.rotation.z);
    }

    private enum DayPart
    {
        Morning,
        Day,
        Evening,
        Night
    }
}
