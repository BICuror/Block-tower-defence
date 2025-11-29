using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using Zenject;
using System;

public class Item : DraggableObject
{
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private GlobalEffectContainer _globalEffectContainer;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    [SerializeField] private ItemColor _itemColor;
    [SerializeField] private VisualEffectHandler _destroyEffectPrefab;
    private List<ToggleGlobalEffectData> _toggleEffectDatas = new();
    private int _duration;
    private int _charges;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private ToggleGlobalEffectData _toggleGlobalEffectData;
    
    [Button]
    public void ApplyEffect()
    {
        if (_toggleEffectDatas.Contains(_toggleGlobalEffectData)) return;
        
        _toggleEffectDatas.Add(_toggleGlobalEffectData);
        
        _globalEffectContainer.AddEffect(_toggleGlobalEffectData);
    }
    
#endif 
    public List<ToggleGlobalEffectData> ToggleEffectDatas => _toggleEffectDatas;
    public int Duration => _duration;
    public int Charges => _charges;
    public ItemColor ItemColor => _itemColor;
    
    public Action<Item> ItemPickedUp;
    public Action<Item> DurationEnded;
    
    private void Awake()
    {
        base.Awake();
        PickedUp += () => ItemPickedUp?.Invoke(this);
    }
    
    public void AddToggleEffectDatas(List<ToggleGlobalEffectData> effectDatas) => _toggleEffectDatas.AddRange(effectDatas); 
    public void SetChargesAmount(int charges) => _charges = charges;
    
    public void SetDuration(int duration)
    {
        _duration = duration;
    }

    public async UniTask DecreaseDuration()
    {
        _duration--;

        if (_duration <= 0)
        {
            await _upgradeChargeContainer.AddChargesWithAnimation(_charges, transform);
            DurationEnded?.Invoke(this);
            DestroyItem();
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
    
    public void DestroyItem()
    {
        Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity).PlayAndForget();
        Destroy(gameObject);
    }
}