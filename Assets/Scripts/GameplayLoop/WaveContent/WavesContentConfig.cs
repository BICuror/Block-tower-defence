using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WavesContentConfig", menuName = "WavesContentConfig")]

public sealed class WavesContentConfig : ScriptableObject
{
    [SerializeField] private List<WaveContent> _waves;
    
    public List<WaveContent> Waves => _waves;

    private void OnValidate()
    {
        _waves.ForEach(wave => wave.SetInspectorIndex(_waves.IndexOf(wave) + 1));
    }
}

[Serializable] public sealed class WaveContent
{
    [HideInInspector] public string Name;
    [SerializeField] List<WaveContentType> _content;
    
    [Header("ItemSetting")]
    [SerializeField] private int _combinedItemStrength = 11;
    [SerializeField] private int _minimalItemStrength = 3;
    [SerializeField] private int _itemsAmount = 2;
    
    public List<WaveContentType> Content => _content;
    
    public int CombinedItemStrength => _combinedItemStrength;
    public int MinimalItemStrength => _minimalItemStrength;
    public int ItemsAmount => _itemsAmount;
    
    public void SetInspectorIndex(int index) => Name = $"Wave {index}";
}

public enum WaveContentType
{
    BuildingSelection,
    FreeBuildingUpgradeSelection,
    BossWave,
}