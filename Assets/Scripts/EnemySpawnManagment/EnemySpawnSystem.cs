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
        
        public void AddSpawner(EnemySpawner spawner)
        {
            _spawners.Add(spawner);
        }
    
        public void RemoveSpawner(EnemySpawner spawner)
        {
            _spawners.Remove(spawner);
        }

        public bool IsAllEnemiesSpawned()
        {
            for (int i = 0; i < _spawners.Count; i++)
            {
                if (_spawners[i].SpawnedAllEnemies() == false) return false;
            }

            return true;
        }
    
        private void CheckIfAllEnemiesDied()
        {
            if (_globalEnemyContainer.Entities.Count > 0) return;
    
            LastWaveEnemyDied.Invoke();
        }
    }
}