using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GlobalEffectData", menuName = "Effects/GlobalEffectData")]

public class GlobalEffectData : InspectableData
{
    [Header("Effect")]
    [SerializeField] private List<InstanceItemTypeContainer> _instanceItemTypeContainers;
    
    [Header("SelectionData")]
    [Range(1, 15)] [SerializeField] private int _quality = 3;
    [SerializeField] private EffectType _effectType;
    [SerializeField] private List<GlobalEffectTag> _tags;
    
    [Header("Stacks")]
    [FormerlySerializedAs("_isUnique")] [SerializeField] private bool _hasStacks;
    [ShowIf("_hasStacks")] [SerializeField] private int _maxStacks;
    
    [Space] [Header("EffectAppearanceCondition")] 
    [SerializeField] private bool _hasEffectAppearanceCondition;
    [AllowNesting] [ShowIf("_hasEffectAppearanceCondition")] [SerializeField] private EffectAppearanceConditionData _effectAppearanceCondition;
    [AllowNesting] [ShowIf("_hasEffectAppearanceCondition")] [SerializeField] private ArgumentsContainer _effectAppearanceConditionArgumentsContainer;
    
    [Header("RequiredTags")]
    [SerializeField] private bool _hasRequiredTags;
    [ShowIf("_hasRequiredTags")] [AllowNesting] [SerializeField] private EntityModifierTagRequirementsContainer _requiredBuildingTags;
    [AllowNesting] [ShowIf("_hasRequiredTags")] [SerializeField] private GlobalEffectTagRequirementContainer _requiredGlobalEffectsTags;
    
    [Header("BlockTags")] 
    [SerializeField] private bool _hasBlockTags;
    [AllowNesting] [ShowIf("_hasBlockTags")] [SerializeField] private EntityModifierTagRequirementsContainer _blockBuildingsTags;
    [AllowNesting] [ShowIf("_hasBlockTags")] [SerializeField] private GlobalEffectTagRequirementContainer _blockGlobalEffectTags;
    
    public List<InstanceItemTypeContainer> InstanceItemTypeContainers => _instanceItemTypeContainers;
    public EffectAppearanceConditionData EffectAppearanceCondition => _effectAppearanceCondition;
    public ArgumentsContainer EffectAppearanceConditionArgumentsContainer => _effectAppearanceConditionArgumentsContainer;
    public List<GlobalEffectTag> Tags => _tags;
    public bool HasAppearanceCondition => _hasEffectAppearanceCondition;
    public int Quality => _quality;
    public bool HasStacks => _hasStacks;
    public int MaxStacks => _maxStacks;
    public EffectType EffectType => _effectType;
    public bool HasRequiredTags => _hasRequiredTags;
    public bool HasBlockTags => _hasBlockTags;
    public List<EntityEffectTagReqirement> RequiredBuildingTags => _requiredBuildingTags.Requirements;
    public List<GlobalEffectTagReqirement> RequiredGlobalEffectsTags => _requiredGlobalEffectsTags.Requirements;
    public List<EntityEffectTagReqirement> BlockBuildingsTags => _blockBuildingsTags.Requirements;
    public List<GlobalEffectTagReqirement> BlockGlobalEffectTags => _blockGlobalEffectTags.Requirements;
    
    public virtual void Modify(GlobalEffect effect) {}
    
    public void SetItemTypeNames(List<string> itemTypeNames)
    {
        _instanceItemTypeContainers.ForEach(itemTypeContainer => itemTypeContainer.AllEffectTypeNames = itemTypeNames);
    }
}

public enum EffectType
{
    Positive,
    Negative,
    Netral
}

public enum GlobalEffectTag
{
    ChangeWaveSize
}