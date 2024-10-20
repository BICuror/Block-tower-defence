using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class ItemFactory : MonoBehaviour
{
    [SerializeField] private Item _itemPrefab;
    [SerializeField] private ItemModifierSelector _modifierSelector;
    [SerializeField] private ItemModifierFactory _itemModifierFactory;

    private void Awake() => CreateItem(0, 1);

    public void CreateItem(int quality, int strength)
    {
        Item item = Instantiate(_itemPrefab, new Vector3(12f, 7f, 12f), Quaternion.identity);

        List<ItemPropertyData> propertyDatas = _modifierSelector.GetProperties(quality, strength);
        item.AddPropertyDatas(propertyDatas);

        List<ItemRewardData> rewardDatas = _modifierSelector.GetRewards(quality, strength);
        item.AddRewardsDatas(rewardDatas);

        List<ItemProperty> properties = new();
        foreach (ItemPropertyData propertyData in propertyDatas)
        {
            properties.Add(_itemModifierFactory.CreateProperty(propertyData.Type));
        }
        item.AddProperties(properties);

        List<ItemReward> rewards = new();
        foreach (ItemRewardData rewardData in rewardDatas)
        {
            rewards.Add(_itemModifierFactory.CreateReward(rewardData.Type));
        }
        item.AddRewards(rewards);
    }
}
