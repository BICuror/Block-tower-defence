using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

public class Item : DraggableObject
{
    [Inject] private GlobalEffectContainer _globalEffectContainer;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    [SerializeField] private List<ToggleGlobalEffectData> _initialRewardEffectDatas;
    
    private List<ToggleGlobalEffectData> _toggleEffectDatas = new();
    private List<RewardGlobalEffectData> _rewardDatas = new();
    
    private List<GlobalRewardEffect> _rewards = new();

    private int _duration;

    public List<RewardGlobalEffectData> RewardDatas => _rewardDatas;
    public List<ToggleGlobalEffectData> ToggleEffectDatas => _toggleEffectDatas;
    public int Duration => _duration;
    
    public Action<Item> ItemPickedUp;
    public Action<Item> DurationEnded;
    
    private void Awake()
    {
        base.Awake();
        PickedUp += OnPickedUp;
        AddToggleEffectDatas(_initialRewardEffectDatas);
    }

    public void AddToggleEffectDatas(List<ToggleGlobalEffectData> effectDatas) => _toggleEffectDatas.AddRange(effectDatas); 
    public void AddRewardEffectDatas(List<RewardGlobalEffectData> rewardDatas) => _rewardDatas.AddRange(rewardDatas);
    
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
    
    public void EnableEffects()
    {
        _globalEffectContainer.AddEffects(_toggleEffectDatas);
        _globalEffectFactory.CreateRewardEffect(_rewardDatas[0]).GrantReward();
    }
    
    public void DisableEffects()
    {
        _globalEffectContainer.RemoveEffects(_toggleEffectDatas);
    }

    private void OnPickedUp() => ItemPickedUp?.Invoke(this);
}