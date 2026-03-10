using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Cashing;
using Combat;

public sealed class Totem : MonoBehaviour
{
    [SerializeField] private Sprite _activeStateIcon;
    [SerializeField] private int _upgradeCharges = 2;
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Cached] private ApplyEffectInArea _applyEffectInArea;
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private EntityCanvas _canvas;
    private EntityCanvasIcon _icon;
    private bool _totemState = true;
    
    private void Start()
    {
        _waveStateController.LastWaveEnemyDied += GrantUpgradeChargesSync;
        _ownerEntity.Activated += ToggleTotemState;

        UpdateIconState();
    }

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
        if (_totemState) _icon = _canvas.AddIcon(_activeStateIcon);
        else _canvas.RemoveIcon(_icon);
    }

    private void GrantUpgradeChargesSync() => GrantUpgradeCharges().Forget();
    private async UniTask GrantUpgradeCharges()
    {
        if (_totemState) await _upgradeChargeContainer.AddChargesWithAnimation(_upgradeCharges, transform);
        
        _ownerEntity.Health.Die();
    }

    private void OnDestroy()
    {
        _waveStateController.LastWaveEnemyDied -= GrantUpgradeChargesSync;
        _ownerEntity.Activated -= ToggleTotemState;
    }
}