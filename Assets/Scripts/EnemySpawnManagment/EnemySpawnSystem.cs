using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Cysharp.Threading.Tasks;

namespace Combat
{
    public sealed class EnemySpawnSystem : MonoBehaviour
    {
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        private List<EnemySpawner> _spawners = new();
        
        public event Action LastWaveEnemyDied;

        public bool AllEnemiesSpawned => !_spawners.Exists(spawner => !spawner.SpawnedAllEnemies);

        private void Awake()
        {
            _globalEnemyContainer.EnemyRemoved += _ => CheckIfAllEnemiesDied();
        }
        
        public void SetEnemyGroupVisibility(bool state)
        {
            if (state) _spawners.ForEach(spawner => spawner.ShowEnemySpawnInfo());   
            else _spawners.ForEach(spawner => spawner.HideEnemySpawnInfo());   
        }
        
        public void StartWave()
        {
            _spawners.ForEach(spawner => spawner.SpawnGroup().Forget());
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
            if (_globalEnemyContainer.Entities.Count > 0) return;

            _spawners.ForEach(spawner => spawner.TrySpawnEnemy());
            
            if (_globalEnemyContainer.Entities.Count > 0 || !AllEnemiesSpawned) return;

            OnAllEnemiesDied();
        }

        private void OnAllEnemiesDied()
        {
            _spawners.ForEach(spawner => spawner.StopSpawning());
            
            LastWaveEnemyDied.Invoke();
        }
    }
}