using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificatorData")]

public class EntityModificatorData : InspectableData
{
    [SerializeField] private bool _showInInspector = true;
    
    [Header("GlobalEffectData")]
    [SerializeField] private List<InstanceItemTypeContainer> _itemTypeContainers;

    [Header("SelectionData")] 
    [SerializeField] private EntityModifcationRarity _rarity;
    [SerializeField] private List<EntityModifcatorTag> _tags;
    [SerializeField] private EffectType _effectType;
    [SerializeField] private bool _hasStacks;
    [AllowNesting] [ShowIf("_hasStacks")] [SerializeField] private int _maxStacks = 1;

    [Header("RequiredTags")]
    [SerializeField] private bool _hasRequiredTags;

    [AllowNesting] [ShowIf("_hasRequiredTags")] [SerializeField] private bool _anyRequired;
    [AllowNesting] [ShowIf("_hasRequiredTags")] [SerializeField] private EntityModifierTagRequirementsContainer _reqiredOwnerTags;
    [AllowNesting] [ShowIf("_hasRequiredTags")] [SerializeField] private EntityModifierTagRequirementsContainer _reqiredOtherEntityTags;

    [Header("BlockTags")] 
    [SerializeField] private bool _hasBlockTags;
    [AllowNesting] [ShowIf("_hasBlockTags")] [SerializeField] private EntityModifierTagRequirementsContainer _blockOwnerTags;
    [AllowNesting] [ShowIf("_hasBlockTags")] [SerializeField] private EntityModifierTagRequirementsContainer _blockOtherEntityTags;

    public List<InstanceItemTypeContainer> ItemTypeContainers => _itemTypeContainers;
    public EffectType EffectType => _effectType;
    public EntityModifcationRarity Rarity => _rarity;
    public List<EntityModifcatorTag> Tags => _tags;
    public bool HasStacks => _hasStacks;
    public int MaxStacks => _maxStacks;
    public bool ShowInInspector => _showInInspector;
    public bool HasRequiredTags => _hasRequiredTags;
    public bool HasBlockTags => _hasBlockTags;
    public List<EntityEffectTagReqirement> ReqiredOwnerTags => _reqiredOwnerTags.Requirements;
    public List<EntityEffectTagReqirement> ReqiredOtherEntityTags => _reqiredOtherEntityTags.Requirements;
    public List<EntityEffectTagReqirement> BlockOwnerTags => _blockOwnerTags.Requirements;
    public List<EntityEffectTagReqirement> BlockOtherEntityTags => _blockOtherEntityTags.Requirements;

    public virtual void Modify(EntityModificator modificator) {}

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
    Heal,
    AppliesNegativeEffect,
    RemovesRecharge,
    ChangesRecharge,
    ItemReroll,
    OverridesMainBehaviour,
    AOE,
    RechargeDownDamageDown,
    RechargeUpDamageUp,
    AreaUp,
    RequiresMark,
    AppliesMark,
}