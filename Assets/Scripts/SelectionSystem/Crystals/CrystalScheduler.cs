using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

public sealed class CrystalScheduler : MonoBehaviour
{
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private ItemFactory _itemFactory;
    [Inject] private WaveManager _waveManager;
    private int _currentWave;
    private int _itemsToSpawn;
    
    private IslandData _islandData => _islandDataContainer.Data;

    private void Awake()
    {
        _globalEnemyContainer.EnemyRemoved += TrySpawnItem;
    }
    
    public void ScheduleCrystals()
    {
        _currentWave = Mathf.Clamp(_waveManager.GetCurrentWave(), 0, _islandData.ItemGenerationConfig.MaxWave);

        _itemsToSpawn = Mathf.RoundToInt(_islandDataContainer.Data.ItemGenerationConfig.AmountCurve.Evaluate(_currentWave));
    }

    private void TrySpawnItem(EnemyEntity enemyEntity)
    {
        if (_globalEnemyContainer.Entities.Count == 0)
        {
            while (_itemsToSpawn > 0)
            {
                SpawnItem(enemyEntity.transform.position);
            }
        }
        else
        {
            if (Random.Range(0f, 1f) < (_itemsToSpawn / _globalEnemyContainer.Entities.Count))
            {
                SpawnItem(enemyEntity.transform.position);
            }
        }
    }

    private void SpawnItem(Vector3 position)
    {
        int itemStrength = Mathf.RoundToInt(_islandData.ItemGenerationConfig.MaxStrengthCurve.Evaluate(_currentWave));
        
        //TEMP FIX
        itemStrength = Random.Range(2, 4);
        
        _itemFactory.CreateItem(itemStrength, position);
        
        _itemsToSpawn--;
    }
}