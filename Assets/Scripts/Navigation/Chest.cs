using WorldGeneration;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class Chest : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private ItemFactory _itemFactory;
    [Inject] private SpawnerRotator _spawnerRotator;

    private void Start()
    {
        _waveStateController.LastWaveEnemyDied += CreateItem;
        _spawnerRotator.RotateSpawner(transform);
        _ownerEntity.Health.Died += Unsubscribe;
    }

    private void CreateItem()
    {
        _itemFactory.CreateItem(1, 1, transform.position);
        Unsubscribe();
        _ownerEntity.Health.Die();
    }

    private void Unsubscribe()
    {
        _waveStateController.LastWaveEnemyDied -= CreateItem;
        _ownerEntity.Health.Died -= Unsubscribe;
    }
}