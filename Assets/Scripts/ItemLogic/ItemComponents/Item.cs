using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

public class Item : DraggableObject
{
    [Inject] private GlobalEffectContainer _globalEffectContainer;
    [Inject] private EffectFactory _effectFactory;
    
    [SerializeField] private List<ToggleEffectData> _initialRewardEffectDatas;
    
    private List<ToggleEffectData> _toggleEffectDatas = new();
    private List<RewardEffectData> _rewardDatas = new();
    
    private List<RewardEffect> _rewards = new();

    private int _duration;
    
    public Action<Item> ItemPickedUp;
    public Action<Item> DurationEnded;
    
    private void Awake()
    {
        base.Awake();
        PickedUp += OnPickedUp;
        AddToggleEffectDatas(_initialRewardEffectDatas);
    }

    public void AddToggleEffectDatas(List<ToggleEffectData> effectDatas) => _toggleEffectDatas.AddRange(effectDatas); 
    public void AddRewardEffectDatas(List<RewardEffectData> rewardDatas) => _rewardDatas.AddRange(rewardDatas);
    
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
        _effectFactory.CreateRewardEffect(_rewardDatas[0]).GrantReward();
    }
    
    public void DisableEffects()
    {
        _globalEffectContainer.RemoveEffects(_toggleEffectDatas);
    }

    private void OnPickedUp() => ItemPickedUp?.Invoke(this);
}