using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using WorldGeneration;

public sealed class EnemySpawnerSystem : MonoBehaviour
{
    [Inject] private WaveManager _waveManager;
    [Inject] private IslandDataContainer _islandDataContainer;
    private IslandData _islandData => _islandDataContainer.Data;

    private List<EnemySpawner> _spawners = new List<EnemySpawner>();

    public UnityEvent<int> GeneratedEnemiesAmount;

    public UnityEvent AllEnemiesDied;

    public UnityEvent<EnemyHealth> EnemySpawned;
    public UnityEvent<EnemyHealth> EnemyDied;
    private float _currentHealthMultiplyer;
    public float HealthMultiplyer => _currentHealthMultiplyer;

    private void InvokeEnemySpawned(EnemyHealth spawnedEnemy) => EnemySpawned.Invoke(spawnedEnemy);
    private void InvokeEnemyDied(EnemyHealth spawnedEnemy) => EnemyDied.Invoke(spawnedEnemy);

    public void StartWave()
    {
        for (int i = 0; i < _spawners.Count; i++)
        {   
            _spawners[i].SpawnGroup();
        }
    }

    public void GenerateEnemyGroups()
    {
        float leftHealthForWave = _islandData.WavesData.WaveHealth * _waveManager.GetCurrentWave();

        int enemiesAmount = 0;

        for (int i = 0; i < _spawners.Count; i++)
        {   
            float leftHealthForGroup = leftHealthForWave / (_spawners.Count - i);

            leftHealthForWave -= leftHealthForGroup;
            
            List<EnemyData> waveGroup = GenerateEnemyGroup(ref leftHealthForGroup);

            leftHealthForWave += leftHealthForGroup;  
            
            enemiesAmount += waveGroup.Count;

            if (i + 1 == _spawners.Count)
            {
                Debug.Log("Spawned enemies: for: " + (_islandData.WavesData.WaveHealth * _waveManager.GetCurrentWave() - leftHealthForWave).ToString() + " out of " + (_islandData.WavesData.WaveHealth * _waveManager.GetCurrentWave()).ToString());
            }

            _spawners[i].SetEnemiesToSpawn(waveGroup);
        }

        GeneratedEnemiesAmount.Invoke(enemiesAmount);
    }

    private List<EnemyData> GenerateEnemyGroup(ref float healthLeft)
    {   
        EnemyWaveGroup waveGroup = FindSutableRandomGroup();

        List<EnemyData> enemiesToSpawn = new List<EnemyData>();
        while (true)
        {
            for (int enemyTypeIndex = 0; enemyTypeIndex < waveGroup.GroupParts.Length; enemyTypeIndex++)
            {
                EnemyWaveGroup.GroupPart currentPart = waveGroup.GroupParts[enemyTypeIndex];
                
                int enemyAmount = Random.Range(currentPart.MinAmount, currentPart.MaxAmount);

                for (int enemyIndex = 0; enemyIndex < enemyAmount; enemyIndex++)
                {
                    if (healthLeft - currentPart.Data.HealthData.MaxHealth >= 0 || enemiesToSpawn.Count == 0)
                    {
                        healthLeft -= currentPart.Data.HealthData.MaxHealth;

                        enemiesToSpawn.Add(currentPart.Data);
                    }
                    else
                    {
                        return enemiesToSpawn;
                    }
                }
            }
        }
    }

    private EnemyWaveGroup FindSutableRandomGroup()
    {
        EnemyWaveGroup[] waveGroups = _islandData.WavesData.WaveGroups;

        List<EnemyWaveGroup> sutableGroups = new List<EnemyWaveGroup>();

        int currentWave = _waveManager.GetCurrentWave();

        for (int i = 0; i < waveGroups.Length; i++)
        {
            if (waveGroups[i].FirstPossibleWaveEncounter <= currentWave && waveGroups[i].LastPossibleWaveEncounter >= currentWave)
            {
                sutableGroups.Add(waveGroups[i]);
            }
        }

        return sutableGroups[Random.Range(0, sutableGroups.Count)];
    }

    public void AddSpawner(EnemySpawner spawner)
    {
        _spawners.Add(spawner);

        spawner.EnemySpawned.AddListener(InvokeEnemySpawned);
        spawner.EnemyDied.AddListener(InvokeEnemyDied);

        spawner.LastEnemyKilled.AddListener(CheckIfAllEnemiesDied);
    }

    public void RemoveSpawner(EnemySpawner spawner)
    {
        _spawners.Remove(spawner);

        spawner.EnemySpawned.RemoveListener(InvokeEnemySpawned);
        spawner.EnemyDied.RemoveListener(InvokeEnemyDied);

        spawner.LastEnemyKilled.RemoveListener(CheckIfAllEnemiesDied);
    }

    private void CheckIfAllEnemiesDied()
    {
        for (int i = 0; i < _spawners.Count; i++)
        {
            if (_spawners[i].AllEnemiesDead() == false || _spawners[i].SpawnedAllEnemies() == false) return;
        }        

        AllEnemiesDied.Invoke();
    }

    public void KillAllEnemies()
    {
        for (int i = 0; i < _spawners.Count; i++)
        {
            _spawners[i].KillAllEnemies();
        }
    }
}
