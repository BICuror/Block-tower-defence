using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificatorData")]

public class EntityModificatorData : ScriptableObject
{
    [Header("GlobalEffectData")]
    [SerializeField] private List<InstanceItemTypeContainer> _itemTypeContainers;
    [SerializeField] private ArgumentsContainer _argumentsContainer;

    [Header("SelectionData")] 
    [SerializeField] private EntityModifcationRarity _rarity;
    [SerializeField] private List<EntityModifcatorTag> _tags;
    
    [Header("UI Data")]
    [SerializeField] private EffectType _effectType;
    [SerializeField] private string _modificatorName;
    [TextArea] [SerializeField] private string _modificatorDescription;
    
    public List<InstanceItemTypeContainer> ItemTypeContainers => _itemTypeContainers;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EffectType EffectType => _effectType;
    
    public EntityModifcationRarity Rarity => _rarity;
    public List<EntityModifcatorTag> Tags => _tags;
    
    public string ModificatorName => _modificatorName;
    public string ModificatorDescription => _modificatorDescription;

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
    MaxHealth,
    Damage,
    Reach,
    Mark,
    Stun,
    Activation,
}