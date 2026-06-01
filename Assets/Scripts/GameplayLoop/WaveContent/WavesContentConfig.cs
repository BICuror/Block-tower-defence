using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WavesContentConfig", menuName = "WavesContentConfig")]

public sealed class WavesContentConfig : ScriptableObject
{
    [SerializeField] private int _minimalItemStrength = 1;
    [SerializeField] private List<WaveContent> _waves;
    
    public int MinimalItemStrength => _minimalItemStrength;
    
    public WaveContent GetWaveContent(int waveIndex) => _waves[waveIndex - 1];
    
    private void OnValidate()
    {
        _waves.ForEach(wave => wave.SetInspectorIndex(_waves.IndexOf(wave) + 1));
    }
}

[Serializable] public sealed class WaveContent
{
    [HideInInspector] public string Name;
    [SerializeField] List<WaveContentType> _content;

    [Header("EnemyWaveGeneration")] 
    [SerializeField] private bool _forceExistingBuildingAttackWaves;
    
    [Header("ItemSetting")]
    [SerializeField] private int _additionalItemStrength = 4;
    [SerializeField] private int _combinedItemStrength = 11;
    [SerializeField] private int _itemsAmount = 2;

    [Header("OptionalTasks")] 
    [SerializeField] private int _optionalTasksAmount = 2;

    [FormerlySerializedAs("_maxEnemySpawnersAmount")]
    [Header("EnemySpawners")] 
    [SerializeField] private int _enemySpawnersAmount = 3;
    
    public List<WaveContentType> Content => _content;
    
    public bool ForceExistingBuildingAttackWaves => _forceExistingBuildingAttackWaves;
    public int AdditionalItemStrength => _additionalItemStrength;
    public int CombinedItemStrength => _combinedItemStrength;
    public int ItemsAmount => _itemsAmount;
    public int OptionalTasksAmount => _optionalTasksAmount;
    public int EnemySpawnersAmount => _enemySpawnersAmount;
    
    public void SetInspectorIndex(int index) => Name = $"Wave {index}";
}

public enum WaveContentType
{
    BuildingSelection = 0,
    FreeBuildingUpgradeSelection = 1,
    BossWave = 2,
    RerollRewardFromOptionalTask = 3,
}