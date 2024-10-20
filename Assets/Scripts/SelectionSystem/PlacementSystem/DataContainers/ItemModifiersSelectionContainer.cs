using UnityEngine;

[CreateAssetMenu(fileName = "ItemModifiersSelectionContainer", menuName = "Item/ItemModifiersSelectionContainer")]

public sealed class ItemModifiersSelectionContainer : ScriptableObject 
{
    [SerializeField] private ItemPropertyContainer _itemPropertyContainer;
    [SerializeField] private ItemRewardContainer _itemRewardContainer;

    public ItemPropertyContainer ItemPropertyContainer => _itemPropertyContainer;
    public ItemRewardContainer ItemRewardContainer => _itemRewardContainer;
}

