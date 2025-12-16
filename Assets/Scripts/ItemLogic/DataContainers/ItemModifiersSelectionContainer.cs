using UnityEngine;

[CreateAssetMenu(fileName = "ItemModifiersSelectionContainer", menuName = "Item/ItemModifiersSelectionContainer")]

public sealed class ItemModifiersSelectionContainer : ScriptableObject 
{
    [SerializeField] private ItemToggleEffectContainer _itemToggleEffectContainer;

    public ItemToggleEffectContainer ItemToggleEffectContainer => _itemToggleEffectContainer;
}