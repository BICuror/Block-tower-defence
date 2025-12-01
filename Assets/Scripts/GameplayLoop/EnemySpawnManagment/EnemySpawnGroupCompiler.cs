using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

using Random = System.Random;

public sealed class EnemySpawnGroupCompiler : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private EnemySpawnSystem _enemySpawnSystem;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private WaveManager _waveManager;
    private Dictionary<EnemySpawner, List<EnemyData>> _enemySpawnDatas = new();
    private List<AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup> _additionalGroups = new();
    private int _currentWaveSeed;
    private int _currentGroupSeed;

    private Random _random = new();
    
    private IslandData _islandData => _islandDataContainer.Data;

    private void Start()
    {
        _waveStateMachine.StateStarted += TryGenerateWaveSeed;
        _globalStatContainer.Get<EnemyAmountMultiplier>().ValueChanged += _ => GenerateEnemyGroups();

        GenerateWaveSeed();
    }

    private void TryGenerateWaveSeed(WaveState waveState)
    {
        if (waveState == WaveState.Idle) GenerateWaveSeed();
    }

    private void GenerateWaveSeed()
    {
        _additionalGroups.Clear();
        _currentWaveSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); 
        _currentGroupSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); 
        GenerateEnemyGroups();
    }

    private void GenerateEnemyGroups()
    {
        GenerateMainEnemyGroups();
        GenerateAdditionalEnemyGroups();
        ApplyEnemyWaveDatas();
    }

    private void GenerateAdditionalEnemyGroups()
    {
        _random = new Random(_currentGroupSeed);

        List<EnemySpawner> spawners = _enemySpawnSystem.Spawners;
        
        _additionalGroups.ForEach(group =>
        {
            EnemySpawner randomEnemySpawner = spawners[_random.Next(0, spawners.Count)];
            
            _enemySpawnDatas[randomEnemySpawner].AddRange(GetEnemyGroupPartAmountModified(group.GroupParts));
        });
    }

    public void AddAdditionalEnemyGroup(AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup additionalEnemyGroup)
    {
        _additionalGroups.Add(additionalEnemyGroup);
        GenerateEnemyGroups();
    }
    
    public void RemoveAdditionalEnemyGroup(AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup additionalEnemyGroup)
    {
        _additionalGroups.Remove(additionalEnemyGroup);
        GenerateEnemyGroups();
    }
    
    private void GenerateMainEnemyGroups()
    {
        _enemySpawnDatas.Clear();
        
        _random = new Random(_currentWaveSeed);
        
        List<EnemySpawner> spawners = _enemySpawnSystem.Spawners;
        
        for (int i = 0; i < spawners.Count; i++)
        {
            List<EnemyData> waveGroup = GetEnemyGroupPartAmountModified(FindSuitableRandomGroup().GroupParts);
            
            _enemySpawnDatas.Add(spawners[i], waveGroup);
        }
    }

    public List<EnemyData> GetEnemyGroupPart(List<EnemyWaveGroup.GroupPart> groupParts, float amountMultiplier)
    {
        List<EnemyData> groupEnemies = new List<EnemyData>();
        
        for (int enemyGroupPartIndex = 0; enemyGroupPartIndex < groupParts.Count; enemyGroupPartIndex++)
        {
            EnemyWaveGroup.GroupPart currentPart = groupParts[enemyGroupPartIndex];

            int enemyAmount = Mathf.RoundToInt(currentPart.GetAmount(_waveManager.GetCurrentWave()) * amountMultiplier);

            for (int enemyIndex = 0; enemyIndex < enemyAmount; enemyIndex++)
            {
                groupEnemies.Add(currentPart.Data);
            }
        }

        return groupEnemies;
    }
    
    private List<EnemyData> GetEnemyGroupPartAmountModified(List<EnemyWaveGroup.GroupPart> groupParts)
    {
        return GetEnemyGroupPart(groupParts, _globalStatContainer.Get<EnemyAmountMultiplier>().Value);
    }

    private EnemyWaveGroup FindSuitableRandomGroup()
    {
        EnemyWaveGroup[] waveGroups = _islandData.WavesData.WaveGroups;

        List<EnemyWaveGroup> suitableGroups = new List<EnemyWaveGroup>();

        int currentWave = _waveManager.GetCurrentWave();

        for (int i = 0; i < waveGroups.Length; i++)
        {
            if (waveGroups[i].FirstPossibleWaveEncounter <= currentWave && waveGroups[i].LastPossibleWaveEncounter >= currentWave)
            {
                suitableGroups.Add(waveGroups[i]);
            }
        }

        return suitableGroups[_random.Next(0, suitableGroups.Count)];
    }

    private void ApplyEnemyWaveDatas()
    {
        List<EnemySpawner> spawners = _enemySpawnSystem.Spawners;
        
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].SetEnemiesToSpawn(_enemySpawnDatas[spawners[i]]);
        }
    }
}