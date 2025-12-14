using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Navigation;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDatas/EnemyData")]

public sealed class EnemyData : ScriptableObject
{
    [Header("Stats")] 
    [SerializeField] private float _contactDamage = 10f;
    [SerializeField] private float _maxHealth = 25f;
    [SerializeField] private float _speed = 2f;

    [SerializeField] private List<StatInitializer> _statInitializer;
    
    [Header("NavigationData")]
    [SerializeField] private NavigationAgentData _navigationData;
    
    [Header("VisualSettings")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;

    [Header("UI")] 
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    
    [Header("Modificators")]
    [SerializeField] private bool _hasObjectModificators;
    [ShowIf("_hasObjectModificators")] [SerializeField] private List<GameObject> _objectModificators;
    
    public float ContactDamage => _contactDamage;
    public float MaxHealth => _maxHealth;
    public float Speed => _speed;
    public List<StatInitializer> StatInitializers => _statInitializer;
    public NavigationAgentData NavigationData => _navigationData;
    public bool HasObjectModificators => _hasObjectModificators;
    public List<GameObject> ObjectModificators => _objectModificators;
    public string Name => _name;
    public string Description => _description;
    
    public Mesh Mesh => _mesh;
    public Material Material => _material;
}
