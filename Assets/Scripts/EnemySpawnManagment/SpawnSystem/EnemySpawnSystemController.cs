using Combat.EnemySpawnSystem.OneAtATime;
using Combat.EnemySpawnSystem.Default;
using Combat.EnemySpawnSystem.Siege;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

namespace Combat.EnemySpawnSystem
{
    public sealed class EnemySpawnSystemController : MonoBehaviour
    {
        [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private GlobalStatContainer _globalStatContainer;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        [Inject] private WaveStateMachine _waveStateMachine;
        
        [SerializeField] private OneAtATimeEnemySpawnSystem _oneAtATimeEnemySpawnSystem;
        [SerializeField] private DefaultEnemySpawnSystem _defaultEnemySpawnSystem;
        [SerializeField] private SiegeEnemySpawnSystem _siegeEnemySpawnSystem;
        [SerializeField] private IdleStateController _idleStateController;
        
        private IEnemySpawnSystem _currentSpawnSystem;
        private StatModifier _enemyAmountModifier = new();
        private List<EnemySpawner> _spawners = new();

        public IReadOnlyList<EnemySpawner> Spawners => _spawners;
        
        public event Action WaveComplete;

        public bool AllEnemiesSpawned => _currentSpawnSystem.AllEnemiesSpawned;
            
        private void Start()
        {
            _globalStatContainer.Get<EnemyAmountMultiplier>().AddStatModifier(_enemyAmountModifier);

            _idleStateController.EnteredStateCompleted += PrepeareWave;
        }

        private void PrepeareWave()
        {
            UpdateCurrentSpawnSystem();
            
            foreach (EnemySpawner spawner in _spawners)
            {
                spawner.SpawnerInfoDisplay.SetSpawnInfo(_enemySpawnGroupContainer.EnemySpawnDatas[spawner]);
            }

            WaveContent content = _waveIndexContainer.GetCurrentWaveContent();
            _currentSpawnSystem.PrepareWave(content, _spawners);
        }
        
        public void SetEnemyGroupVisibility(bool state)
        {
            if (state) _spawners.ForEach(spawner => spawner.SpawnerInfoDisplay.ShowSpawnInfo());   
            else _spawners.ForEach(spawner => spawner.SpawnerInfoDisplay.HideSpawnInfo());   
        }

        public void UpdateEnemySpawnMultiplier()
        {
            _enemyAmountModifier.SetMultiplier(_waveIndexContainer.GetCurrentWaveContent().EnemySpawnAmountMultiplier);
        }
        
        public async UniTask StartWave()
        {
            WaveContent content = _waveIndexContainer.GetCurrentWaveContent();
            await _currentSpawnSystem.SpawnWave(content, _spawners);
            
            WaveComplete?.Invoke();
        }
        
        public void AddSpawner(EnemySpawner spawner)
        {
            _spawners.Add(spawner);
        }
    
        public void RemoveSpawner(EnemySpawner spawner)
        {
            _spawners.Remove(spawner);
        }

        private void UpdateCurrentSpawnSystem()
        {
            WaveContent content = _waveIndexContainer.GetCurrentWaveContent();
            
            switch (content.SpawnMethod)
            {
                case EnemySpawnMethod.Default: _currentSpawnSystem = _defaultEnemySpawnSystem; break;
                case EnemySpawnMethod.OneSpawnerAtATime: _currentSpawnSystem = _oneAtATimeEnemySpawnSystem; break;
                case EnemySpawnMethod.Siege: _currentSpawnSystem = _siegeEnemySpawnSystem; break;
            }
        }
    }
}