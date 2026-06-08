using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyWaveGroup", menuName = "SpawnManagement/EnemyWaveDefaultGroupWeightContainer")]

public sealed class EnemyWaveDefaultGroupWeightContainer : ScriptableObject
{
    [SerializeField] private EnemyGroupWeightContainer enemyGroupWeightContainer;
    
    public EnemyGroupWeightContainer EnemyGroupWeightContainer => enemyGroupWeightContainer;
    
    [Button] public void ParseEnemyGroupWeightContainer()
    { 
        enemyGroupWeightContainer.ParseEnemyGroupWeightContainer();
    }
}

[Serializable] public struct EnemyGroupWeightContainer
{
    [SerializeField] private List<int> _amountPerWave;
    
    [Header("Parsing")] 
    [SerializeField] private string _parseString;

    public int GetAmount(int waveIndex)
    {
        if (waveIndex >= _amountPerWave.Count) return _amountPerWave[^1];
            
        return _amountPerWave[waveIndex];
    }

    public void ParseEnemyGroupWeightContainer()
    {
        if (string.IsNullOrEmpty(_parseString)) return;
            
        string currentString = _parseString;

        int index = 0;
        _amountPerWave.Clear();
        _amountPerWave.Add(1);
            
        string[] split = currentString.Split(' ');  
            
        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].IndexOf(' ') >= 0) split[i] = split[i].Remove(split[i].IndexOf(' '));
            if (!string.IsNullOrEmpty(split[i])) _amountPerWave.Add(int.Parse(split[i]));
        }
    }
}