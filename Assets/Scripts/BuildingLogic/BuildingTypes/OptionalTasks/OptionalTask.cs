using UnityEngine;
using Zenject;
using Cashing;
using Combat;

public abstract class OptionalTask : MonoBehaviour
{
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Cached] protected CombatEntity OwnerEntity;
    
    [SerializeField] private int _chargesToSpawn = 2;

    protected void Start()
    {
        _waveStateController.LastWaveEnemyDied += RewardChargesSync;
        OwnerEntity.Health.Died += OnDeath;
    }

    protected abstract bool IsCompleted();

    private void RewardChargesSync() => RewardCharges();
    private async void RewardCharges()
    {
        if (IsCompleted())
        {
            await _upgradeChargeContainer.AddChargesWithAnimation(_chargesToSpawn, transform);
        }
        
        OwnerEntity.Health.Die();
    }

    private void OnDeath()
    {
        _waveStateController.LastWaveEnemyDied -= RewardChargesSync;
        OwnerEntity.Health.Died -= OnDeath;
    }
}