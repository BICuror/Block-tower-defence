using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Zenject;
using WorldGeneration;

public sealed class WaveManager : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    private IslandData _islandData => _islandDataContainer.Data;
    [Inject] private EnemySpawnerSystem _enemySpawnerSystem;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
    [Inject] private RoadGenerator _roadGenerator;
    [Inject] private EnemyBiomeGenerator _enemyBiomeGenerator;
    [Inject] private RoadNodeGenerator _roadNodeGenerator;
    [Inject] private IslandDecorationContainer _decorationContainer;
    [Inject] private EnemyNavigator _enemyNavigator;

    [SerializeField] private float _waveEndingDuration;
    [SerializeField] private float _wavePreparationDuration;

    private int _currentWave;
    
    public UnityEvent<float> WaveStopped;
    public UnityEvent<float> WavePreparationBegun;

    public UnityEvent WaveStarted;

    public UnityEvent WavesEnded;

    private bool _waveIsReadyToBeStarted;

    private void Awake() => _roadNodeGenerator.SetupNodes();

    public int GetCurrentWave() => _currentWave;
    
    public void StopWave()
    {
        WaveStopped.Invoke(_waveEndingDuration);

        _enemyBiomesContainer.DisableBiomesTerrain(_waveEndingDuration);
        _enemyBiomesContainer.IncreaseBiomesStages();

        _islandData.ReachedWave(GetCurrentWave());

        if (_currentWave < _islandData.MaxWave) StartCoroutine(WaitToStopWave());
        else WavesEnded.Invoke();
    }
    
    private IEnumerator WaitToStopWave()
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
        _roadGenerator.GenerateRoads();
        _enemyNavigator.GenerateNodeMap();
        _enemyBiomesContainer.RegenerateBiomes();
        _enemyBiomesContainer.GenerateBiomesDecorations();
        _enemyBiomesContainer.EnableBiomesTerrain(_wavePreparationDuration);

        WavePreparationBegun.Invoke(_wavePreparationDuration);
        _enemySpawnerSystem.GenerateEnemyGroups();
        StartCoroutine(WaitToPrepeareWave());
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
