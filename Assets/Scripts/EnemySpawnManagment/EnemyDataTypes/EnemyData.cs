using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Navigation;
using CuroAudio;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDatas/EnemyData")]

public sealed class EnemyData : ScriptableObject
{
    [Header("Stats")] 
    [SerializeField] private EnemyTier _tier;
    [SerializeField] private BuildingAttackType _buildingAttackType = BuildingAttackType.Group;
    [SerializeField] private float _spawnDelay = 0.65f;
    [SerializeField] private bool _diesOnContact;
    [SerializeField] private float _contactDamage = 10f;
    [SerializeField] private float _maxHealth = 25f;
    [SerializeField] private float _speed = 2f;

    [SerializeField] private List<StatInitializer> _statInitializer;
    
    [Header("EffectImmunities")]
    [SerializeField] private List<string> _effectImmunities;
    
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

    [Header("Audio")] 
    [SerializeField] private AudioEnum _deathSound = AudioEnum.sound_enemy_death;

    public EnemyTier Tier => _tier;
    public BuildingAttackType BuildingAttackType => _buildingAttackType;
    public float SpawnDelay => _spawnDelay;
    public bool DiesOnContact => _diesOnContact;
    public float ContactDamage => _contactDamage;
    public float MaxHealth => _maxHealth;
    public float Speed => _speed;
    public List<StatInitializer> StatInitializers => _statInitializer;
    public List<string> EffectImmunities => _effectImmunities;
    public NavigationAgentData NavigationData => _navigationData;
    public bool HasEntityModificators => _hasEntityModificators;
    public List<EntityModificatorData> EntityModificatorDatas => _entityModificatorDatas;
    public bool HasObjectModificators => _hasObjectModificators;
    public List<GameObject> ObjectModificators => _objectModificators;
    public string LocalizationKey => _localizationKey;
    
    public Mesh Mesh => _mesh;
    public Material Material => _material;
    public float Scale => _scale;
    public AudioEnum DeathSound => _deathSound;
}

//Used for enemy tier selection when generating enemy waves
public enum EnemyTier
{
    Tier_1,
    Tier_2,
    Tier_3,
}