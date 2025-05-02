using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

public class Item : DraggableObject
{
    [Inject] private GlobalEffectContainer _globalEffectContainer;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    [SerializeField] private GameObject _destroyEffectPrefab;
    private List<ToggleGlobalEffectData> _toggleEffectDatas = new();
    private List<RewardGlobalEffectData> _rewardDatas = new();
    private int _duration;
    private int _quality;
    private int _strength;

    public List<RewardGlobalEffectData> RewardDatas => _rewardDatas;
    public List<ToggleGlobalEffectData> ToggleEffectDatas => _toggleEffectDatas;
    public int Duration => _duration;
    public int Quality => _quality;
    public int Strength => _strength;
    
    public Action<Item> ItemPickedUp;
    public Action<Item> DurationEnded;
    
    private void Awake()
    {
        base.Awake();
        PickedUp += () => ItemPickedUp?.Invoke(this);
    }

    public void AddToggleEffectDatas(List<ToggleGlobalEffectData> effectDatas) => _toggleEffectDatas.AddRange(effectDatas); 
    public void AddRewardEffectDatas(List<RewardGlobalEffectData> rewardDatas) => _rewardDatas.AddRange(rewardDatas);

    public void SetItemData(int quality, int strength)
    {
        _quality = quality;
        _strength = strength;
    }
    
    public void SetDuration(int duration)
    {
        _duration = duration;
    }

    public void DecreaseDuration()
    {
        _duration--;

        if (_duration <= 0)
        {
            GrantRewardEffect();
            DurationEnded?.Invoke(this);
            Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    public void EnableToggleEffects()
    {
        _globalEffectContainer.AddEffects(_toggleEffectDatas);
    }
    
    public void DisableToggleEffects()
    {
        _globalEffectContainer.RemoveEffects(_toggleEffectDatas);
    }

    public void GrantRewardEffect()
    {
        _rewardDatas.ForEach(rewardEffectData =>
        {
            _globalEffectFactory.CreateRewardEffect(rewardEffectData).GrantReward();
        });
    }
}