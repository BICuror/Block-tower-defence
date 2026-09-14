using System.Collections.Generic;
using System.Linq;
using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

using Random = System.Random;

public sealed class EnemySpawnGroupCompiler : MonoBehaviour
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [Inject] private WaveIndexContainer _waveIndexContainer;
    [Inject] private WaveStateMachine _waveStateMachine;
    private Dictionary<EnemySpawner, List<EnemyData>> _enemySpawnDatas = new();
    private List<AdditionalEnemyGroupData> _additionalGroups = new();
    private List<AdditionalEnemyGroupData> _additionalWaveGroups = new();
    private bool _forceSpawnExistingBuildingAttackTypeGroups;
    private int _currentWaveSeed;
    private int _currentGroupSeed;

    private Random _random = new();
    
    private IslandData _islandData => _islandDataContainer.Data;
    public int CurrentWaveSeed => _currentWaveSeed;

    private void Start()
    {
        _globalStatContainer.Get<EnemyAmountMultiplier>().ValueChanged += _ => GenerateEnemyGroups();
    }

    public List<EnemyData> GetAllEnemyDatas()
    {
        List<EnemyData> result = new();
        
        _additionalGroups.ForEach(group => group.GroupParts.ForEach(groupPart => result.Add(groupPart.Data)));
        
        foreach (var enemySpawnData in _enemySpawnDatas)
        {
            result.AddRange(enemySpawnData.Value);
        }
        
        return result;
    }

    public void RegenerateWaveSeed()
    {
        _currentWaveSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); 
        _currentGroupSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
    
    public void SetNextWaveData(bool forceExistingBuildingAttackWaves)
    {
        _additionalGroups.Clear();

        _forceSpawnExistingBuildingAttackTypeGroups = forceExistingBuildingAttackWaves;
    }

    public void GenerateEnemyGroups()
    {
        GenerateMainEnemyGroups();
        GenerateAdditionalEnemyGroups();
        ApplyEnemyWaveDatas();
    }

    private void GenerateAdditionalEnemyGroups()
    {
        _random = new Random(_currentGroupSeed);
        
        _additionalGroups.ForEach(group =>
        {
            EnemySpawner randomEnemySpawner = _enemyBiomeContainer.EnemyBiomeList[_random.Next(0, _enemyBiomeContainer.EnemyBiomeList.Count)].EnemySpawner;
            
            _enemySpawnDatas[randomEnemySpawner].AddRange(GetEnemyGroupPartAmountModified(group.GroupParts));
        });
    }

    public void AddAdditionalEnemyGroup(AdditionalEnemyGroupData additionalEnemyGroup)
    {
        _additionalGroups.Add(additionalEnemyGroup);
        GenerateEnemyGroups();
    }
    
    public void RemoveAdditionalEnemyGroup(AdditionalEnemyGroupData additionalEnemyGroup)
    {
        _additionalGroups.Remove(additionalEnemyGroup);
        GenerateEnemyGroups();
    }
    
    private void GenerateMainEnemyGroups()
    {
        _enemySpawnDatas.Clear();
        
        _random = new Random(_currentWaveSeed);
        
        for (int i = 0; i < _enemyBiomeContainer.EnemyBiomeList.Count; i++)
        {
            EnemyBiome currentBiome = _enemyBiomeContainer.EnemyBiomeList[i];
            
            List<EnemyData> waveGroup = GetEnemyGroupPartAmountModified(FindSuitableRandomGroup().GroupParts);
            
            _enemySpawnDatas.Add(currentBiome.EnemySpawner, waveGroup);
        }
    }
    
    public List<EnemyData> GetEnemyGroupPartAmountModified(List<EnemyGroupPart> groupParts)
    {
        return GetEnemyGroupPart(groupParts, _globalStatContainer.Get<EnemyAmountMultiplier>().Value);
    }

    public List<EnemyData> GetEnemyGroupPart(List<EnemyGroupPart> groupParts, float amountMultiplier)
    {
        List<EnemyData> groupEnemies = new List<EnemyData>();
        
        for (int enemyGroupPartIndex = 0; enemyGroupPartIndex < groupParts.Count; enemyGroupPartIndex++)
        {
            EnemyGroupPart currentPart = groupParts[enemyGroupPartIndex];

            int enemyAmount = currentPart.GetEnemyAmount(_waveIndexContainer.GetCurrentWave());

            if (currentPart.ScaleByModificators) enemyAmount = Mathf.RoundToInt(enemyAmount * amountMultiplier);
            
            if (enemyAmount <= 0) enemyAmount = 1;

            for (int enemyIndex = 0; enemyIndex < enemyAmount; enemyIndex++)
            {
                groupEnemies.Add(currentPart.Data);
            }
        }

        return groupEnemies;
    }

    private EnemyWaveGroup FindSuitableRandomGroup()
    {
        EnemyWaveGroup[] waveGroups = _islandData.WavesData.WaveGroups;
        
        List<EnemyWaveGroup> suitableGroups = new List<EnemyWaveGroup>();

        for (int i = 0; i < waveGroups.Length; i++)
        {
            if (CanSpawnWaveGroup(waveGroups[i]))
            {
                suitableGroups.Add(waveGroups[i]);
            }
        }

        return suitableGroups[_random.Next(0, suitableGroups.Count)];
    }

    private void ApplyEnemyWaveDatas()
    {
        for (int i = 0; i < _enemyBiomeContainer.EnemyBiomeList.Count; i++)
        {
            EnemySpawner spanwer = _enemyBiomeContainer.EnemyBiomeList[i].EnemySpawner;
            
            spanwer.SetEnemiesToSpawn(_enemySpawnDatas[spanwer]);
        }
    }

    private bool CanSpawnWaveGroup(EnemyWaveGroup group)
    {
        WaveContent content = _waveIndexContainer.GetCurrentWaveContent();

        if (content.RequiredEnemyTier != group.WaveGroupTier) return false;

        if (!_forceSpawnExistingBuildingAttackTypeGroups) return true;
     
        List<BuildingAttackType> existingBuildingAttackTypes = new();
        
        _globalBuildingContainer.GetPlayerBuildings().ForEach(buildingEntity =>
        {
            if (!existingBuildingAttackTypes.Contains(buildingEntity.BuildingAttackType) && buildingEntity.BuildingAttackType != BuildingAttackType.None)
            {
                existingBuildingAttackTypes.Add(buildingEntity.BuildingAttackType);
            }
        });

        return existingBuildingAttackTypes.Contains(group.BuildingAttackType);
    }
}