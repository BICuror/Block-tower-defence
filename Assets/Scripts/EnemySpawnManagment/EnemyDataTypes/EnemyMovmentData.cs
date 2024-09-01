using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMovmentData", menuName = "EnemyDatas/EnemyMovmentData")]

public sealed class EnemyMovmentData : ScriptableObject 
{
    [SerializeField] private float _movmentSpeed;
    public float MovmentSpeed => _movmentSpeed;
}
