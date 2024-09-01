using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDatas/EnemyData")]

public sealed class EnemyData : ScriptableObject 
{
    [SerializeField] private EnemyMovmentData _enemyMovmentData;
    public EnemyMovmentData MovmentData => _enemyMovmentData;

    [SerializeField] private EnemyHealthData _enemyHealthData;
    public EnemyHealthData HealthData => _enemyHealthData;

    [Header("VisualSettings")]
    [SerializeField] private Mesh _mesh;
    public Mesh GetMesh() => _mesh;

    [SerializeField] private Material _material;
    public Material GetMaterial() => _material;

    [SerializeField] private SpecialEnemyObject _specialObject;
    public SpecialEnemyObject GetSpecialObject() => _specialObject;
}
