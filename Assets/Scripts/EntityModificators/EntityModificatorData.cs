using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificators/EntityModificatorData")]

public class EntityModificatorData : InspectableData
{
    [Space] [Header("GlobalEffectData")]
    [SerializeField] private List<InstanceItemTypeContainer> _itemTypeContainers;

    [Space] [Header("AppearLogic")] 
    [Tooltip("Whether tags should include specified buildings or exclude them")]
    [SerializeField] private bool _tagsInclude = true;
    [SerializeField] private List<BuildingEntityType> _buildingEntityTypeTags;

    [Space] [Header("SelectionData")] 
    [SerializeField] private EntityModifcationRarity _rarity;
    [SerializeField] private List<EntityModifcatorTag> _tags;
    [SerializeField] private bool _hasStacks;
    [AllowNesting] [ShowIf("_hasStacks")] [SerializeField] private int _maxStacks = 1;
    
    [Space] [Header("UIData")] 
    [SerializeField] private EffectType _effectType;
    [SerializeField] private bool _showInInspector = true; 
    
    [Space] [Header("RequiredTags")]
    [SerializeField] private bool _hasRequiredTags;

    [AllowNesting] [ShowIf("_hasRequiredTags")] [SerializeField] private EntityModifierTagRequirementsContainer _reqiredOwnerTags;
    [AllowNesting] [ShowIf("_hasRequiredTags")] [SerializeField] private EntityModifierTagRequirementsContainer _reqiredOtherEntityTags;

    [Header("BlockTags")] 
    [SerializeField] private bool _hasBlockTags;
    [AllowNesting] [ShowIf("_hasBlockTags")] [SerializeField] private EntityModifierTagRequirementsContainer _blockOwnerTags;
    [AllowNesting] [ShowIf("_hasBlockTags")] [SerializeField] private EntityModifierTagRequirementsContainer _blockOtherEntityTags;

    [Header("RequiredUpgrades")] 
    [SerializeField] private bool _hasRequiredUpgrades;
    [AllowNesting] [ShowIf("_hasRequiredUpgrades")] [SerializeField] private List<EntityModificatorData> _requiredUpgrades;
    
    [Space] [Header("StatChanges")]
    [SerializeField] private List<StatChange> _statChanges;
    [SerializeField] private List<StatInitializer> _statInitializers;

    public List<InstanceItemTypeContainer> ItemTypeContainers => _itemTypeContainers;
    public bool BuildingTagsInclude => _tagsInclude;
    public List<BuildingEntityType> BuildingEntityTypeTags => _buildingEntityTypeTags;
    public EntityModifcationRarity Rarity => _rarity;
    public List<EntityModifcatorTag> Tags => _tags;
    public bool HasStacks => _hasStacks;
    public int MaxStacks => _maxStacks;
    public EffectType EffectType => _effectType;
    public bool ShowInInspector => _showInInspector;
    public bool HasRequiredTags => _hasRequiredTags;
    public bool HasBlockTags => _hasBlockTags;
    public List<EntityEffectTagReqirement> ReqiredOwnerTags => _reqiredOwnerTags.Requirements;
    public List<EntityEffectTagReqirement> ReqiredOtherEntityTags => _reqiredOtherEntityTags.Requirements;
    public List<EntityEffectTagReqirement> BlockOwnerTags => _blockOwnerTags.Requirements;
    public List<EntityEffectTagReqirement> BlockOtherEntityTags => _blockOtherEntityTags.Requirements;
    public bool HasRequiredUpgrades => _hasRequiredUpgrades;
    public List<EntityModificatorData> RequiredUpgrades => _requiredUpgrades;
    public List<StatChange> StatChanges => _statChanges;
    public List<StatInitializer> StatInitializers => _statInitializers;
    
    public void SetItemTypeNames(List<string> itemTypeNames)
    {
        _itemTypeContainers.ForEach(itemTypeContainer => itemTypeContainer.AllEffectTypeNames = itemTypeNames);
    }
}

public enum EntityModifcationRarity
{
    Common,
    Rare,
    Legendary
}

public enum EntityModifcatorTag
{
    HealingRelated = 0,
    NegativeEffectRelated = 1,
    RemovesRecharge = 2,
    ExplosionRelated = 3,
    ItemReroll = 4,
    OverridesMainBehaviour = 5,
    AOERelated = 6,
    BuildingExsists = 7,
    AreaUpgrade = 9,
    RequiresMark = 10,
    AppliesMark = 11,
    SelfHarm = 12,
    NonLethal = 13,
    MaxEntitiesUpgrade = 14,
    MultipleActionInvoke = 16,
    DeathRelated = 17,
    TemporaryTowersRelated = 18,
    OnBuildRelated = 19,
    DamageUpgrade = 20,
    RechargeUpgrade = 21,
    BuildTimeUpgrade = 22,
    HealthUpgrade = 23,
    ExplosionRadiusUpgrade = 24,
    HasDamage = 25,
    HasExplosionDamage = 26,
    HasRecharge = 27,
    HasReachArea = 28,
}