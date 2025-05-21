using System.Collections.Generic;
using System.Linq;
using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

public sealed class EnemySpawnGroupCompiler : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private WaveManager _waveManager;
    private Dictionary<EnemySpawner, List<EnemyData>> _enemySpawnDatas = new();
    private List<AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup> _additionalGroups = new();
    private int _currentWaveSeed;
    private int _currentGroupSeed;
    
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
        _currentWaveSeed = Random.Range(int.MinValue, int.MaxValue); 
        _currentGroupSeed = Random.Range(int.MinValue, int.MaxValue); 
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
        Random.InitState(_currentGroupSeed);
        
        _additionalGroups.ForEach(group =>
        {
            EnemySpawner randomEnemySpawner = _enemyBiomeContainer.EnemyBiomeList[Random.Range(0, _enemyBiomeContainer.EnemyBiomeList.Count)].EnemySpawner;
            
            float groupHealth = group.GroupHealth;
            
            _enemySpawnDatas[randomEnemySpawner].AddRange(GetEnemyGroupPart(group.GroupParts, ref groupHealth));
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
        
        Random.InitState(_currentWaveSeed);
        
        for (int i = 0; i < _enemyBiomeContainer.EnemyBiomeList.Count; i++)
        {
            EnemyBiome currentBiome = _enemyBiomeContainer.EnemyBiomeList[i];
            
            float waveHealth = _islandData.WavesData.WaveHealth * _waveManager.GetCurrentWave() * _globalStatContainer.Get<EnemyAmountMultiplier>().Value;
            
            List<EnemyData> waveGroup = GenerateEnemyGroup(waveHealth, FindSuitableRandomGroup());
            
            _enemySpawnDatas.Add(currentBiome.EnemySpawner, waveGroup);
        }
    }

    private List<EnemyData> GenerateEnemyGroup(float waveHealth, EnemyWaveGroup waveGroup)
    {   
        List<EnemyData> enemiesToSpawn = new List<EnemyData>();
        
        while (waveHealth > 0)
        {
            enemiesToSpawn.AddRange(GetEnemyGroupPart(waveGroup.GroupParts, ref waveHealth));
        }
        
        return enemiesToSpawn;
    }

    private List<EnemyData> GetEnemyGroupPart(List<EnemyWaveGroup.GroupPart> groupParts, ref float leftHealth)
    {
        List<EnemyData> groupEnemies = new List<EnemyData>();
        
        for (int enemyGroupPartIndex = 0; enemyGroupPartIndex < groupParts.Count; enemyGroupPartIndex++)
        {
            EnemyWaveGroup.GroupPart currentPart = groupParts[enemyGroupPartIndex];
            
            int enemyAmount = Random.Range(currentPart.MinAmount, currentPart.MaxAmount);

            for (int enemyIndex = 0; enemyIndex < enemyAmount; enemyIndex++)
            {
                if (leftHealth - currentPart.Data.MaxHealth >= 0 || groupEnemies.Count == 0)
                {
                    leftHealth -= currentPart.Data.MaxHealth;

                    groupEnemies.Add(currentPart.Data);
                }
                else
                {
                    return groupEnemies;
                }
            }
        }

        return groupEnemies;
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

        return suitableGroups[Random.Range(0, suitableGroups.Count)];
    }

    private void ApplyEnemyWaveDatas()
    {
        for (int i = 0; i < _enemyBiomeContainer.EnemyBiomeList.Count; i++)
        {
            EnemySpawner spanwer = _enemyBiomeContainer.EnemyBiomeList[i].EnemySpawner;
            
            spanwer.SetEnemiesToSpawn(_enemySpawnDatas[spanwer]);
        }
    }
}