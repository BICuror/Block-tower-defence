using System.Collections.Generic;
using ModestTree;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveGroup", menuName = "SpawnManagement/EnemyWaveGroup")]

public sealed class EnemyWaveGroup : ScriptableObject
{
    [SerializeField] private List<GroupPart> _groupParts;
    [SerializeField] private int _firstPossibleWaveEncounter, _lastPossibleWaveEncounter;
    
    public List<GroupPart> GroupParts => _groupParts;
    public int FirstPossibleWaveEncounter => _firstPossibleWaveEncounter;
    public int LastPossibleWaveEncounter => _lastPossibleWaveEncounter;
    
    [Button] private void ParseAll()
    {
        _groupParts.ForEach(part => part.ParseString());
    }
    
    [System.Serializable] public struct GroupPart
    {
        [SerializeField] private List<int> _amountPerWave;
        [SerializeField] private EnemyData _enemyData;
        
        public EnemyData Data => _enemyData;

        public int GetAmount(int wave)
        {
            Debug.Log(_enemyData);
            Debug.Log(wave);
            
            if (wave <= 0) return _amountPerWave[0];
            if (wave >= _amountPerWave.Count) return _amountPerWave[^1];
            
            return _amountPerWave[wave];
        }

        [Header("Parsing")] 
        [SerializeField] private string _parseString;

        public void ParseString()
        {
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
}