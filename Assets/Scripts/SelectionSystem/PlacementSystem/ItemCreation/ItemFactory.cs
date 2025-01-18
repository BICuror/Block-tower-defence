using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class ItemFactory : MonoBehaviour
{
    [Inject] private DraggableCreator _draggableCreator;
    
    [SerializeField] private Item _itemPrefab;
    [SerializeField] private ItemModifierSelector _modifierSelector;
    [SerializeField] private ItemModifierFactory _itemModifierFactory;
    
    public void CreateItem(int quality, int strength, Vector3 position)
    {
        Item item = Instantiate(_itemPrefab, new Vector3(12f, 7f, 12f), Quaternion.identity);

        int duration = 2;//Random.Range(1, 4);
        item.SetDuration(duration);
        
        List<ItemPropertyData> propertyDatas = _modifierSelector.GetProperties(quality, Mathf.Min(1, Mathf.RoundToInt(strength / (float)duration)));
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
