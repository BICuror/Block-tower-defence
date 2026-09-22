using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Combat
{
    public sealed class EnemySpawnSystem : MonoBehaviour
    {
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private GlobalStatContainer _globalStatContainer;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        private List<EnemySpawner> _spawners = new();
        private StatModifier _enemyAmountModifier = new();
        
        public event Action LastWaveEnemyDied;

        public bool AllEnemiesSpawned => !_spawners.Exists(spawner => !spawner.SpawnedAllEnemies);
            
        private void Start()
        {
            _globalStatContainer.Get<EnemyAmountMultiplier>().AddStatModifier(_enemyAmountModifier);
            _globalEnemyContainer.EnemyRemoved += _ => CheckIfAllEnemiesDied();
        }
        
        public void SetEnemyGroupVisibility(bool state)
        {
            if (state) _spawners.ForEach(spawner => spawner.ShowEnemySpawnInfo());   
            else _spawners.ForEach(spawner => spawner.HideEnemySpawnInfo());   
        }

        public void UpdateEnemySpawnMultiplier()
        {
            _enemyAmountModifier.SetMultiplier(_waveIndexContainer.GetCurrentWaveContent().EnemySpawnAmountMultiplier);
        }
        
        public async UniTask StartWave()
        {
            _spawners.ForEach(spanwer => spanwer.HideEnemySpawnInfo());

            WaveContent content = _waveIndexContainer.GetCurrentWaveContent();
            
            if (content.SpawnMethod == EnemySpawnMethod.Simultaneous)
            {
                _spawners.ForEach(spawner => spawner.SpawnGroup(content.SpawnDelayMultiplier).Forget());
            }
            else if (content.SpawnMethod == EnemySpawnMethod.OneSpawnerAtATime)
            {
                EnemySpawner previousSpawner = _spawners[UnityEngine.Random.Range(0, _spawners.Count)];
                
                while (!AllEnemiesSpawned)
                {
                    EnemySpawner nextSpawner;

                    List<EnemySpawner> remainingSpawners = _spawners.Where(spawner => !spawner.SpawnedAllEnemies).ToList();

                    if (remainingSpawners.Count > 1)
                    {
                        remainingSpawners.Remove(previousSpawner);
                        nextSpawner = remainingSpawners[Random.Range(0, remainingSpawners.Count)];
                    }
                    else nextSpawner = remainingSpawners[0];
                    
                    previousSpawner = nextSpawner;

                    await nextSpawner.SpawnGroup(content.SpawnDelayMultiplier, content.TimeBeforeSwitchingToOtherSpawner);
                }
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