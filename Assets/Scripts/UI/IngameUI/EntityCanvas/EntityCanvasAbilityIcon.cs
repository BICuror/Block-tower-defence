using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

public sealed class EntityCanvasAbilityIcon : ProgressBarBase
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private WaveState _activeWaveState;
    private bool _hasActiveWaveState;
    private float _previousValue;
    
    public bool IsActive => gameObject.activeSelf;
    protected override string ProgressFieldName => "BuildProgress";
    
    public Action StateUpdated;
    
    public void Initialize(Sprite sprite, float value = 0)
    {
        Initialize();
        _spriteRenderer.sprite = sprite;
        _previousValue = value;
        
        SetValue(value).Forget();
    }

    public async void SetActiveWaveState(WaveState waveState)
    {
        _activeWaveState = waveState;
        _hasActiveWaveState = true;
        
        await UniTask.WaitUntil(() => _waveStateMachine != null);
        
        UpdateState(_waveStateMachine.CurrentState);
        _waveStateMachine.StateStarted += UpdateState;
    }

    public async UniTask SetValue(float value)
    {
        await FillBar(_previousValue, value, 1f);
        _previousValue = value;
    }

    private void UpdateState(WaveState waveState)
    {
        if (!_hasActiveWaveState) return;
        
        gameObject.SetActive(waveState == _activeWaveState);
        
        StateUpdated?.Invoke();
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        
        _waveStateMachine.StateStarted -= UpdateState;
    }
}