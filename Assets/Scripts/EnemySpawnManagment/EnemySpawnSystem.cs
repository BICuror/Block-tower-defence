using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;
using System;

using Random = UnityEngine.Random;

namespace Combat
{
    public sealed class EnemySpawnSystem : MonoBehaviour
    {
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveManager _waveManager;
        private List<EnemySpawner> _spawners = new();
        
        private IslandData _islandData => _islandDataContainer.Data;
        
        public Action LastWaveEnemyDied;

        private void Awake()
        {
            _globalEnemyContainer.EnemyRemoved += _ => CheckIfAllEnemiesDied();
        }
        
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
                    Debug.Log($"Spawned enemies: for: {_islandData.WavesData.WaveHealth * _waveManager.GetCurrentWave() - leftHealthForWave} out of {_islandData.WavesData.WaveHealth * _waveManager.GetCurrentWave()}");
                }
    
                _spawners[i].SetEnemiesToSpawn(waveGroup);
            }
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
                        if (healthLeft - currentPart.Data.MaxHealth >= 0 || enemiesToSpawn.Count == 0)
                        {
                            healthLeft -= currentPart.Data.MaxHealth;
    
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
        }
    
        public void RemoveSpawner(EnemySpawner spawner)
        {
            _spawners.Remove(spawner);
        }
    
        private void CheckIfAllEnemiesDied()
        {
            for (int i = 0; i < _spawners.Count; i++)
            {
                if (_spawners[i].SpawnedAllEnemies() == false) return;
            }        
    
            LastWaveEnemyDied.Invoke();
        }
    }
}