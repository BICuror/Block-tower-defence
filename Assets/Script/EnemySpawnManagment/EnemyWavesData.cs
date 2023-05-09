using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWavesData", menuName = "BlockTowerDefence/EnemyWavesData")]

public class EnemyWavesData : ScriptableObject
{
    [SerializeField] private EnemyGroup[] _possibleGroups;
    public EnemyGroup[] PossibleGroups {get => _possibleGroups;}
    
    [System.Serializable] public struct EnemyGroup
    {
        public EnemyInGroup[] EnemiesInGroup;
        public float Weight;
    }

    [System.Serializable] public struct EnemyInGroup
    {
        public EnemyType Type;
        public int Amount;
    }

    [System.Serializable] public struct Enemy
    {
        public GameObject Prefab;
        public float Weight;
    }
    
    public Enemy[] GetEnemy(EnemyType type)
    {
        Enemy[] enemies = type switch 
        {
            EnemyType.Default => _defaultEnemies,
            EnemyType.Tank => _tankEnemies,
            EnemyType.Ranger => _rangerEnemies,
            EnemyType.Support => _supportEnemies,
            EnemyType.Healer => _healerEnemies,
            EnemyType.Quickie => _quickieEnemies
        };

        return enemies;
    }

    [SerializeField] private Enemy[] _defaultEnemies;
    public Enemy[] DefaultEnemies {get => _defaultEnemies;}

    [SerializeField] private Enemy[] _tankEnemies;
    public Enemy[] TankEnemies {get => _tankEnemies;}

    [SerializeField] private Enemy[] _rangerEnemies;
    public Enemy[] RangerEnemies {get => _rangerEnemies;}

    [SerializeField] private Enemy[] _supportEnemies;
    public Enemy[] SupportEnemies {get => _supportEnemies;}

    [SerializeField] private Enemy[] _healerEnemies;
    public Enemy[] HealerEnemies {get => _healerEnemies;}

    [SerializeField] private Enemy[] _quickieEnemies;
    public Enemy[] QuickieEnemies {get => _quickieEnemies;}
}
