using System.Collections.Generic;
using Ligofff.CustomSOIcons;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GlobalEffectData : ScriptableObject
{
    [Header("Effect")]
    [SerializeField] private List<InstanceItemTypeContainer> _instanceItemTypeContainers;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    
    [Header("SelectionData")]
    [Range(1, 15)] [SerializeField] private int _quality = 3;
    [SerializeField] private EffectType _effectType;
    [SerializeField] private bool _isUnique;
    [SerializeField] private List<GlobalEffectTag> _tags;

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

    [Space] [Header("TooltipData")] 
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _effectName;
    [TextArea] [SerializeField] private string _effectDescription;
    
    public List<InstanceItemTypeContainer> InstanceItemTypeContainers => _instanceItemTypeContainers;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectAppearanceConditionData EffectAppearanceCondition => _effectAppearanceCondition;
    public ArgumentsContainer EffectAppearanceConditionArgumentsContainer => _effectAppearanceConditionArgumentsContainer;
    public List<GlobalEffectTag> Tags => _tags;
    public bool HasAppearanceCondition => _hasEffectAppearanceCondition;
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    public EffectType EffectType => _effectType;
    [CustomAssetIcon] public Sprite Icon => _icon;
    public string EffectName => _effectName;
    public string EffectDescription => _effectDescription;
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