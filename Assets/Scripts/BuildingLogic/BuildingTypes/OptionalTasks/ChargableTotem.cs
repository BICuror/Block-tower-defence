using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using Zenject;
using System;
using Combat;

public sealed class ChargableTotem : OptionalTask
{
    [Header("UI")]
    [SerializeField] private Sprite _chargeIcon;
    [SerializeField] private EntityCanvasBar _chargeBarPrefab;
    
    [Header("Charge")]
    [SerializeField] private float _requiredChargePerEnemy;
    [SerializeField] private float _requiredChargePerEnemyForTile;
    
    [Inject] private UpgradeChargeContainer _chargeContainer;
    [Cached] private EntityCanvas _entityCanvas;
    [Cached] private AreaEntityDetector _entityDetector;
    
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private EntityCanvasBar _chargeBar;
    private float _requiredCharge;
    private float _currentCharge;
    private bool _isCharging;
    
    private void Start()
    {
        base.Start();
        
        _entityDetector.AddedItem += TryStartGainingCharge;
        _entityDetector.RemovedItem += TryStopGainingCharge;
        
        _chargeBar = _entityCanvas.AddBar(_chargeIcon, 0f, _chargeBarPrefab);
    }

    protected override bool IsCompleted() => _currentCharge >= _requiredCharge;

    public void CalculateRequiredCharge(int incomingEnemiesCount, int tilesInArea) => _requiredCharge = incomingEnemiesCount * tilesInArea * _requiredChargePerEnemyForTile;

    private void TryStartGainingCharge(CombatEntity _)
    {
        if (!_isCharging && !_entityDetector.IsEmpty) Charge().Forget();
    }

    private void TryStopGainingCharge(CombatEntity _)
    {
        if (_isCharging && _entityDetector.IsEmpty) StopCharging();
    }

    private async UniTask Charge()
    {
        _isCharging = true;

        while (true)
        {
            try
            {
                await UniTask.WaitForSeconds(1f, cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                break;
            }

            GainCharge(_entityDetector.Count);
        }
        
        _isCharging = false;
    }

    private void StopCharging()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void GainCharge(float amount)
    {
        _currentCharge += amount;
        
        _chargeBar.SetValue(_currentCharge / _requiredCharge).Forget();
    }

    private void OnDestroy()
    {
        _entityDetector.AddedItem -= TryStartGainingCharge;
        _entityDetector.RemovedItem -= TryStopGainingCharge;
    }
}