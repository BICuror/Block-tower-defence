using UnityEngine;
using System;
using System.Collections.Generic;

public class Item : DraggableObject
{
    private List<ItemPropertyData> _propertyDatas = new();
    private List<ItemRewardData> _rewardDatas = new();

    private List<ItemProperty> _properties = new();
    private List<ItemReward> _rewards = new();

    public Action<Item> ItemPickedUp;

    private void Awake()
    {
        base.Awake();
        PickedUp.AddListener(OnPickedUp);
    }

    public void AddPropertyDatas(List<ItemPropertyData> propertyDatas) => _propertyDatas.AddRange(propertyDatas);
    public void AddRewardsDatas(List<ItemRewardData> rewardDatas) => _rewardDatas.AddRange(rewardDatas);

    public void AddProperties(List<ItemProperty> properties) => _properties.AddRange(properties);
    public void AddRewards(List<ItemReward> rewards) => _rewards.AddRange(rewards);

    private void OnPickedUp() => ItemPickedUp?.Invoke(this);
}