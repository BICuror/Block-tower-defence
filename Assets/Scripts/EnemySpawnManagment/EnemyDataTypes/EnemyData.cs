using System.Collections.Generic;
using CuroLocalization;
using NaughtyAttributes;
using UnityEngine;
using Navigation;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDatas/EnemyData")]

public sealed class EnemyData : ScriptableObject
{
    [Header("Stats")] 
    [SerializeField] private bool _diesOnContact;
    [SerializeField] private float _contactDamage = 10f;
    [SerializeField] private float _maxHealth = 25f;
    [SerializeField] private float _speed = 2f;

    [SerializeField] private List<StatInitializer> _statInitializer;
    
    [Header("NavigationData")]
    [SerializeField] private NavigationAgentData _navigationData;
    
    [Header("VisualSettings")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private float _scale = 1f;

    [Header("UI")] 
    [SerializeField] private string _localizationKey;

    [Header("Modificators")] 
    [SerializeField] private bool _hasEntityModificators;
    [ShowIf("_hasEntityModificators")] [SerializeField] private List<EntityModificatorData> _entityModificatorDatas;
    
    [SerializeField] private bool _hasObjectModificators;
    [ShowIf("_hasObjectModificators")] [SerializeField] private List<GameObject> _objectModificators;

    public bool DiesOnContact => _diesOnContact;
    public float ContactDamage => _contactDamage;
    public float MaxHealth => _maxHealth;
    public float Speed => _speed;
    public List<StatInitializer> StatInitializers => _statInitializer;
    public NavigationAgentData NavigationData => _navigationData;
    public bool HasEntityModificators => _hasEntityModificators;
    public List<EntityModificatorData> EntityModificatorDatas => _entityModificatorDatas;
    public bool HasObjectModificators => _hasObjectModificators;
    public List<GameObject> ObjectModificators => _objectModificators;
    public string LocalizationKey => _localizationKey;
    
    public Mesh Mesh => _mesh;
    public Material Material => _material;
    public float Scale => _scale;
}
