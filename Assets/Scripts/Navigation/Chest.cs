using WorldGeneration;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class Chest : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private ItemFactory _itemFactory;
    [Inject] private SpawnerRotator _spawnerRotator;

    private void Start()
    {
        _waveStateController.LastWaveEnemyDied += CreateItem;
        _spawnerRotator.RotateSpawner(transform);
        _ownerEntity.Health.Died += Unsubscribe;
    }

    private async void CreateItem()
    {
        Unsubscribe();

        await _upgradeChargeContainer.AddChargesWithAnimation(Random.Range(3, 5), transform);
        
        _ownerEntity.Health.Die();
    }

    private void Unsubscribe()
    {
        _waveStateController.LastWaveEnemyDied -= CreateItem;
        _ownerEntity.Health.Died -= Unsubscribe;
    }
}