using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Zenject;
using WorldGeneration;
using Navigation;

namespace Combat
{
    public sealed class WaveManager : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        
        [Inject] private EnemySpawnerSystem _enemySpawnerSystem;
        [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
        [Inject] private RoadGenerator _roadGenerator;
        [Inject] private EnemyBiomeGenerator _enemyBiomeGenerator;
        [Inject] private RoadNodeGenerator _roadNodeGenerator;
        [Inject] private IslandDecorationContainer _decorationContainer;
        [Inject] private RoadMapGenerator _roadMapGenerator;
        [Inject] private OptionalTaskGenerator _optionalTaskGenerator; 
        [Inject] private NavigationMapGenerator _navigationMapGenerator; 
    
        [SerializeField] private float _waveEndingDuration;
        [SerializeField] private float _wavePreparationDuration;
    
        private int _currentWave;
        private bool _waveIsReadyToBeStarted;
        
        private IslandData _islandData => _islandDataContainer.Data;
        
        public UnityEvent<float> WaveStopped;
        public UnityEvent<float> WavePreparationBegun;
    
        public UnityEvent WaveStarted;
        private void Awake() => _roadNodeGenerator.SetupNodes();
    
        public int GetCurrentWave() => _currentWave;
        
        public void EndWave()
        {
            WaveStopped.Invoke(_waveEndingDuration);
    
            _enemyBiomesContainer.DisableBiomesTerrain(_waveEndingDuration);
            _enemyBiomesContainer.IncreaseBiomesStages();
    
            StartCoroutine(WaitToEndWave());
        }
        
        private IEnumerator WaitToEndWave()
        {
            yield return new WaitForSeconds(_waveEndingDuration);
    
            PrepeareWave();
        }   
    
        public void PrepeareWave()
        {     
            _currentWave++;
    
            if (_currentWave != 1) _decorationContainer.ActivateAllDecorations();
            _enemyBiomesContainer.DestroyOldBiomes();
            _enemyBiomeGenerator.TryGenerateNewBiome();
            _roadMapGenerator.GenerateRoads();
            _optionalTaskGenerator.GenerateTasksAndModifyRoadMap();
            _navigationMapGenerator.GenerateMap();
            _roadGenerator.GenerateRoads();
            _enemyBiomesContainer.RegenerateBiomes();
            _enemyBiomesContainer.GenerateBiomesDecorations();
            _enemyBiomesContainer.EnableBiomesTerrain(_wavePreparationDuration);
            _enemySpawnerSystem.GenerateEnemyGroups();
            
            StartCoroutine(WaitToPrepeareWave());
            
            WavePreparationBegun.Invoke(_wavePreparationDuration);
        }
    
        private IEnumerator WaitToPrepeareWave()
        {
            yield return new WaitForSeconds(_wavePreparationDuration);
        
            TryToStartWave();
        }   
    
        public void TryToStartWave()
        {
            if (_waveIsReadyToBeStarted == false)
            {
                _waveIsReadyToBeStarted = true;
            }
            else
            {
                _waveIsReadyToBeStarted = false;
    
                StartWave();
            }
        }
    
        private void StartWave()
        {
            WaveStarted.Invoke();
    
            _enemySpawnerSystem.StartWave();
        }
    }
}