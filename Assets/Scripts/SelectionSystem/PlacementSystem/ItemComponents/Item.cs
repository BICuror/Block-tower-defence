using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : DraggableObject
{
    [SerializeField] private ItemModifierFactory _itemModifierFactory;
    
    [SerializeField] private List<ItemPropertyData> _initialProperties;
    
    private List<ItemPropertyData> _propertyDatas = new();
    private List<ItemRewardData> _rewardDatas = new();

    private List<ItemProperty> _properties = new();
    private List<ItemReward> _rewards = new();

    private int _duration;
    
    public Action<Item> ItemPickedUp;
    public Action<Item> DurationEnded;
    
    private void Awake()
    {
        PickedUp += OnPickedUp;
    }

    private void Start()
    {
        List<ItemProperty> properties = new();
        
        _initialProperties.ForEach(propertyData =>
        {
            properties.Add(_itemModifierFactory.CreateProperty(propertyData.Type));
        });
        
        AddPropertyDatas(_initialProperties);
        AddProperties(properties);
    }

    public void AddPropertyDatas(List<ItemPropertyData> propertyDatas) => _propertyDatas.AddRange(propertyDatas);
    public void AddRewardsDatas(List<ItemRewardData> rewardDatas) => _rewardDatas.AddRange(rewardDatas);

    public void AddProperties(List<ItemProperty> properties) => _properties.AddRange(properties);
    public void AddRewards(List<ItemReward> rewards) => _rewards.AddRange(rewards);

    public void SetDuration(int duration)
    {
        Debug.Log($"Duration used to be: {_duration}");
        _duration = duration;
        Debug.Log($"Duration set to: {_duration}");
    }

    public void DecreaseDuration()
    {
        _duration--;

        if (_duration <= 0)
        {
            DurationEnded?.Invoke(this);
            Destroy(gameObject);
        }
    }
    
    public void EnableProperties()
    {
        _properties.ForEach(property =>
        {
            property.Enable();
        });
    }
    
    public void DisableProperties()
    {
        _properties.ForEach(property =>
        {
            property.Disable();
        });
    }

    private void OnPickedUp() => ItemPickedUp?.Invoke(this);
}