using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using DG.Tweening;

public sealed class EntityCanvasAbilityIcon : ProgressBarBase
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [Header("Scale")]
    [SerializeField] private float _defaultScale = 1f;
    [SerializeField] private float _depletedScale;

    private float _fillDuration = 1f;
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
    
    public void SetFillDuration(float fillDuration) => _fillDuration = fillDuration;

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
        transform.DOKill();
        
        if (value != 0 )transform.DOScale(_defaultScale, 0.5f).SetLink(gameObject);
        
        await FillBar(_previousValue, value, _fillDuration);
        _previousValue = value;
        
        if (value == 0) transform.DOScale(_depletedScale, 0.5f).SetLink(gameObject);
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