using UnityEngine;
using Zenject;
using WorldGeneration;
using System.Collections.Generic;

public sealed class CrystalCreator : MonoBehaviour
{
    [Inject] private DraggableCreator _draggableCreator;
    [Inject] private EnemySpawnerSystem _enemySpawnerSystem;
    [Inject] private IslandDataContainer _islandDataContainer;
    private IslandData _islandData => _islandDataContainer.Data;
    
    [Header("CrystalPrefabs")]
    [SerializeField] private Crystal _waveCrystal;
    [SerializeField] private Crystal _buildingSelectionCrystal;
    [SerializeField] private Crystal _upgradeSelectionCrystal;

    private int _waveCrystalSpawnIndex;
    private Dictionary<int, CrystalType> _spawnSchedule;
    private int _currentIndex;

    private void Awake()
    {
        _enemySpawnerSystem.GeneratedEnemiesAmount.AddListener(GenerateSpawnSchedule);
        _enemySpawnerSystem.EnemyDied.AddListener(MoveSchedule);
    }

    private void MoveSchedule(EnemyHealth enemyHealth)
    {
        if (_spawnSchedule.ContainsKey(_currentIndex))
        { 
            _draggableCreator.CreateDraggableOnRandomPosition(GetCrystalPrefab(_spawnSchedule[_currentIndex]), enemyHealth.transform.position, 2, null, GetCrystalPrefab(_spawnSchedule[_currentIndex]).GetLauncher());
        }
        
        if (_currentIndex == _waveCrystalSpawnIndex)
        {
            _draggableCreator.CreateDraggableOnRandomPosition(_waveCrystal, enemyHealth.transform.position, 2, null, _waveCrystal.GetLauncher());
        }

        _currentIndex++;
    }

    private Crystal GetCrystalPrefab(CrystalType type)
    {
        switch (type)
        {
            case CrystalType.Wave: return _waveCrystal;
            case CrystalType.Building: return _buildingSelectionCrystal;
            case CrystalType.Upgrade: return _upgradeSelectionCrystal;
        }

        return _waveCrystal;
    }

    private void GenerateSpawnSchedule(int enemiesAmount)
    {
        _currentIndex = 0;
        _spawnSchedule = new Dictionary<int, CrystalType>();

        

        List<CrystalType> typesToGenerate = new List<CrystalType>();

        for (int i = 0; i < _islandData.CrystalSettings.SpawnSettings.Length; i++)
        {
            if (Random.Range(0f, 1f) <= _islandData.CrystalSettings.SpawnSettings[i].SpawnChanse)
            {
                typesToGenerate.Add(_islandData.CrystalSettings.SpawnSettings[i].Type);

                if (typesToGenerate.Count >= enemiesAmount) break;
            }
        }

        if (typesToGenerate.Count == 0)
        {
            _waveCrystalSpawnIndex = Random.Range(0, enemiesAmount);
        }
        else 
        {
            _waveCrystalSpawnIndex = -1;
        }

        List<int> leftIndexes = new List<int>();

        for (int i = 0; i < enemiesAmount; i++) leftIndexes.Add(i);

        for (int i = 0; i < typesToGenerate.Count; i++)
        {
            int randomIndex = Random.Range(0, leftIndexes.Count);

            _spawnSchedule.Add(leftIndexes[randomIndex], typesToGenerate[i]);

            leftIndexes.RemoveAt(randomIndex);
        }
    }
}
