using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class EnemySpawner : MonoBehaviour
{
    private EnemyWavesData.EnemyGroup _currentGroup;

    public UnityEvent<GameObject> EnemySpawned;

    private int _currentEnemyTypeIndex;

    private int _leftToSpawn;

    private float _currentDelay;

    private List<GameObject> _spawnedEnemies;

    public UnityEvent LastEnemyKilled;

    private EnemyWavesData _enemyWavesData;

    private float _currentWheightMultyplier;

    private float _leftWeights;

    private float _groupMultiplier;

    private void Awake()
    {
        FindObjectOfType<EnemySpawnerSystem>().AddSpawner(this);

        _spawnedEnemies = new List<GameObject>();

        LastEnemyKilled.AddListener(StopAllCoroutines);
        LastEnemyKilled.AddListener(TrySpawnEnemy);
    }

    private void OnDestroy() => FindObjectOfType<EnemySpawnerSystem>().RemoveSpawner(this);

    public bool AllEnemiesDead() => _spawnedEnemies.Count == 0;

    public void SpawnGroup(EnemyWavesData enemyWavesData, EnemyWavesData.EnemyGroup group, float delay, float weightMultiplier, float groupMultiplier)
    {
        _currentWheightMultyplier = weightMultiplier;
        _enemyWavesData = enemyWavesData;
        _currentGroup = group;
        _currentEnemyTypeIndex = -1;
        _currentDelay = delay;
        _groupMultiplier = groupMultiplier;

        TrySpawnEnemy();
    }

    private IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(_currentDelay);

        TrySpawnEnemy();
    }   

    private void TrySpawnEnemy()
    {
        if (_currentEnemyTypeIndex < _currentGroup.EnemiesInGroup.Length)
        {
            if (_leftToSpawn <= 0) 
            {
                _currentEnemyTypeIndex++;

                if (_currentEnemyTypeIndex < _currentGroup.EnemiesInGroup.Length)
                {
                    _leftToSpawn = Mathf.RoundToInt(_currentGroup.EnemiesInGroup[_currentEnemyTypeIndex].Amount * _groupMultiplier);
                    _leftWeights = _currentGroup.Weight * _currentWheightMultyplier;
                    TrySpawnEnemy();
                }
            }
            else 
            {
                _leftToSpawn--;

                SpawnEnemy();
                
                StartCoroutine(WaitToSpawn());
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemyToSpawn = GetRandomEnemyPrefab();
     
        GameObject spawnedEnemy = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);

        _spawnedEnemies.Add(spawnedEnemy);

        spawnedEnemy.GetComponent<EnemyHealth>().DeathEvent.AddListener(RemoveEnemy);

        EnemySpawned.Invoke(spawnedEnemy);
    }

    private GameObject GetRandomEnemyPrefab()
    {
        EnemyWavesData.Enemy[] possibleEnemies = _enemyWavesData.GetEnemy(_currentGroup.EnemiesInGroup[_currentEnemyTypeIndex].Type);

        for (int i = possibleEnemies.Length - 1; i >= 0; i++)
        {
            if (possibleEnemies[i].Weight >= _leftWeights)
            {
                _leftWeights -= possibleEnemies[i].Weight;

                return possibleEnemies[i].Prefab;
            }
        }

        return possibleEnemies[0].Prefab;
    }

    private void RemoveEnemy(EnemyHealth enemyHealth)
    {
        _spawnedEnemies.Remove(enemyHealth.gameObject);

        if (_spawnedEnemies.Count == 0) LastEnemyKilled?.Invoke();
    }
}
