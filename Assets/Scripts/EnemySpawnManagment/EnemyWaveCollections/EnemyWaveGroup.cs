using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveGroup", menuName = "SpawnManagement/EnemyWaveGroup")]

public sealed class EnemyWaveGroup : ScriptableObject 
{    
    [SerializeField] private GroupPart[] _groupParts;
    public GroupPart[] GroupParts => _groupParts;
    
    [SerializeField] private int _firstPossibleWaveEncounter, _lastPossibleWaveEncounter;
    public int FirstPossibleWaveEncounter => _firstPossibleWaveEncounter;
    public int LastPossibleWaveEncounter => _lastPossibleWaveEncounter;

    [System.Serializable] public struct GroupPart
    {
        [SerializeField] private EnemyData _enemyData;
        public EnemyData Data => _enemyData;

        [SerializeField] private int _minAmount, _maxAmount;
        public int MinAmount => _minAmount;
        public int MaxAmount => _maxAmount;
    }
}