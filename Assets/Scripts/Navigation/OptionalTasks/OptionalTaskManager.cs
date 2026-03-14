using System.Collections.Generic;
using WorldGeneration;
using System.Linq;
using UnityEngine;
using Navigation;
using Zenject;

public sealed class OptionalTaskManager : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [Inject] private WaveIndexContainer _waveIndexContainer;
    [SerializeField] private List<OptionalTaskGenerator> _optionalTaskGenerators;
    private List<AdditionalTaskLayerPrebuildData> _layerPrebuildDatas = new();

    public List<AdditionalTaskLayerPrebuildData> LayerPrebuildDatas => _layerPrebuildDatas;

    public void GenerateTasksAndModifyRoadMap()
    {
        int tasksToGenerate = GetRequiredTaskCount();
        
        _layerPrebuildDatas.Clear();

        _optionalTaskGenerators = _optionalTaskGenerators.OrderBy(generator => Random.Range(0, _optionalTaskGenerators.Count)).ToList();

        List<Vector2Int> spawnerPositions = _enemyBiomeContainer.GetEnemyBiomesPositions();
        int tasksGenerated = 0;
        
        for (int spawnerIndex = 0; spawnerIndex < spawnerPositions.Count; spawnerIndex++)
        {
            for (int taskGeneratorIndex = 0; taskGeneratorIndex < _optionalTaskGenerators.Count; taskGeneratorIndex++)
            {
                if (_optionalTaskGenerators[taskGeneratorIndex].TryGenerateOptionalTask(spawnerPositions[spawnerIndex], out AdditionalTaskLayerPrebuildData layerPrebuildData))
                {
                    if (layerPrebuildData != null)
                    {
                        _layerPrebuildDatas.Add(layerPrebuildData);
                    }

                    tasksGenerated++;

                    break;
                }
            }
            
            if (tasksGenerated >= tasksToGenerate) break;
        }
    }

    private int GetRequiredTaskCount()
    {
        WavesContentConfig wavesContentConfig = _islandDataContainer.Data.WavesContentConfig;

        return wavesContentConfig.GetWaveContent(_waveIndexContainer.GetCurrentWave()).OptionalTasksAmount;
    }
}