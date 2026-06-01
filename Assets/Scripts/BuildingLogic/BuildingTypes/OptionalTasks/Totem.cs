using UnityEngine;
using Zenject;
using Cashing;
using Cysharp.Threading.Tasks;

public sealed class Totem : OptionalTask
{
    [SerializeField] private VisualEffectHandler _visualEffectHandler;
    [SerializeField] private Sprite _nonactiveStateIcon;
    [SerializeField] private Sprite _activeStateIcon;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Cached] private ApplyEffectInArea _applyEffectInArea;
    private EntityCanvasIcon _icon;
    private bool _totemState = true;
    
    private void Start()
    {
        base.Start();
        
        OwnerEntity.Activated += ToggleTotemState;

        _icon = EntityCanvas.AddIcon(_activeStateIcon);
    }

    protected override bool IsCompleted() => _totemState;

    private void ToggleTotemState()
    {
        if (_waveStateMachine.CurrentState == WaveState.Attack) return;

        if (_totemState) _applyEffectInArea.Disable();
        else _applyEffectInArea.Enable();
        
        _totemState = !_totemState;
        UpdateIconState();
    }

    private void UpdateIconState()
    {
        if (_totemState)
        {
            _visualEffectHandler.PlayEffect();
            _icon.SetIcon(_activeStateIcon);
        }
        else
        {
            _visualEffectHandler.StopPermamentEffect().Forget();
            _icon.SetIcon(_nonactiveStateIcon);
        }
    }

    private void OnDestroy()
    {
        OwnerEntity.Activated -= ToggleTotemState;
    }
}