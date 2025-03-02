using UnityEngine;

[CreateAssetMenu(fileName = "ItemModifiersSelectionContainer", menuName = "Item/ItemModifiersSelectionContainer")]

public sealed class ItemModifiersSelectionContainer : ScriptableObject 
{
    [SerializeField] private ItemToggleEffectContainer _itemToggleEffectContainer;
    [SerializeField] private ItemRewardEffectCotainer _itemRewardEffectContainer;

    public ItemToggleEffectContainer ItemToggleEffectContainer => _itemToggleEffectContainer;
    public ItemRewardEffectCotainer ItemRewardEffectCotainer => _itemRewardEffectContainer;
}