using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class EnemySpawnerSystem : MonoBehaviour
{
    [SerializeField] private WaveManager _waveManager;

    private List<EnemySpawner> _spawners = new List<EnemySpawner>();
    
    [SerializeField] private EnemyWavesData _enemyWavesData;

    public UnityEvent AllEnemiesDied;

    public UnityEvent<GameObject> EnemySpawned;

    public void AddSpawner(EnemySpawner spawner)
    {
        _spawners.Add(spawner);

        spawner.EnemySpawned.AddListener(InvokeEnemySpawned);

        spawner.LastEnemyKilled.AddListener(CheckIfAllEnemiesDied);
    }

    public void RemoveSpawner(EnemySpawner spawner)
    {
        _spawners.Remove(spawner);

        spawner.EnemySpawned.RemoveListener(InvokeEnemySpawned);

        spawner.LastEnemyKilled.RemoveListener(CheckIfAllEnemiesDied);
    }

    private void InvokeEnemySpawned(GameObject spawnedEnemy) => EnemySpawned?.Invoke(spawnedEnemy);

    private void CheckIfAllEnemiesDied()
    {
        for (int i = 0; i < _spawners.Count; i++)
        {
            if (_spawners[i].AllEnemiesDead() == false) return;
        }        

        AllEnemiesDied?.Invoke();
    }

    public void StartWave()
    {
        for (int i = 0; i < _spawners.Count; i++)
        {
            _spawners[i].SpawnGroup(_enemyWavesData, _enemyWavesData.PossibleGroups[Random.Range(0, _enemyWavesData.PossibleGroups.Length)], 1f, 2f, (_waveManager.GetCurrentWave() / 3f) + 1f);
        }
    }
}
