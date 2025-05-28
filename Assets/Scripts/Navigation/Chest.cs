using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

public sealed class Chest : MonoBehaviour
{
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private ItemFactory _itemFactory;
    [Inject] private SpawnerRotator _spawnerRotator;

    private void Awake()
    {
        _waveStateController.LastWaveEnemyDied += CreateItem;
        _spawnerRotator.RotateSpawner(transform);
    }

    private void CreateItem()
    {
        _itemFactory.CreateItem(1, 2, transform.position);
            
        _waveStateController.LastWaveEnemyDied -= CreateItem;
        
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _waveStateController.LastWaveEnemyDied -= CreateItem;
    }
}