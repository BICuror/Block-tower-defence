using UnityEngine;

public sealed class TimeTower : MonoBehaviour, IActivatable
{
    [SerializeField] private float[] _timeScaleStates;

    private int _currentScaleState;

    private void Awake() => SetTimeScale();   

    public void Activate()
    {
        _currentScaleState++;

        if (_currentScaleState == _timeScaleStates.Length) _currentScaleState = 0;

        SetTimeScale();
    }

    private void SetTimeScale() => Time.timeScale = _timeScaleStates[_currentScaleState];
}
