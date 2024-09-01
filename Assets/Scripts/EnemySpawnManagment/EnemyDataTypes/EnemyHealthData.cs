using UnityEngine;

[CreateAssetMenu(fileName = "EnemyHealthData", menuName = "EnemyDatas/EnemyHealthData")]

public sealed class EnemyHealthData : ScriptableObject 
{
    [SerializeField] private float _maxHealth;
    public float MaxHealth => _maxHealth;

    [Range(0.01f, 3f)] [SerializeField] private float _incomingDamageMultipluer;
    public float IncomingDamageMultipluer => _incomingDamageMultipluer;
}