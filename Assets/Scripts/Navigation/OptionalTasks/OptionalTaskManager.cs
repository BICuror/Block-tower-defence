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
        List<OptionalTaskRewardType> rewardTypes = GetRewardTypes(tasksToGenerate);
        
        _layerPrebuildDatas.Clear();

        _optionalTaskGenerators = _optionalTaskGenerators.OrderBy(generator => Random.Range(0, _optionalTaskGenerators.Count)).ToList();

        List<Vector2Int> spawnerPositions = _enemyBiomeContainer.GetEnemyBiomesPositions();
        int tasksGenerated = 0;
        
        for (int spawnerIndex = 0; spawnerIndex < spawnerPositions.Count && tasksGenerated < tasksToGenerate; spawnerIndex++)
        {
            for (int taskGeneratorIndex = 0; taskGeneratorIndex < _optionalTaskGenerators.Count; taskGeneratorIndex++)
            {
                if (_optionalTaskGenerators[taskGeneratorIndex].TryGenerateOptionalTask(spawnerPositions[spawnerIndex], out AdditionalTaskLayerPrebuildData layerPrebuildData, out OptionalTask optionalTask))
                {
                    if (layerPrebuildData != null)
                    {
                        _layerPrebuildDatas.Add(layerPrebuildData);
                    }

                    optionalTask.SetRewardType(rewardTypes[spawnerIndex]);
                    
                    tasksGenerated++;

                    break;
                }
            }
        }
    }

    private List<OptionalTaskRewardType> GetRewardTypes(int amount)
    {
        List<OptionalTaskRewardType> rewardTypes = new();

        int rerollsAmount = _waveIndexContainer.GetCurrentWaveContent().Content.Count(type => type == WaveContentType.RerollRewardFromOptionalTask);

        for (int i = 0; i < rerollsAmount && i < amount; i++)
        {
            rewardTypes.Add(OptionalTaskRewardType.Reroll);
        }

        for (int i = rerollsAmount; i < amount; i++)
        {
            rewardTypes.Add(OptionalTaskRewardType.UpgradeCharges);
        }
        
        return rewardTypes;
    }

    private int GetRequiredTaskCount()
    {
        return _waveIndexContainer.GetCurrentWaveContent().OptionalTasksAmount;
    }
}