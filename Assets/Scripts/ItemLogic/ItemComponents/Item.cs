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
    public int Charges => _charges;
    public ItemColor ItemColor => _itemColor;
    
    public event Action<Item> ItemPickedUp;
    public event Action<Item> ItemDestroyed;
    
    private void Awake()
    {
        base.Awake();
        PickedUp += () => ItemPickedUp?.Invoke(this);
    }
    
    public void AddToggleEffectDatas(List<ToggleGlobalEffectData> effectDatas) => _toggleEffectDatas.AddRange(effectDatas); 
    public void SetChargesAmount(int charges) => _charges = charges;

    public async UniTask DecreaseDuration()
    {
        if (_charges == 0) await UniTask.WaitForSeconds(1f);
        
        await _upgradeChargeContainer.AddChargesWithAnimation(_charges, transform);
        
        DestroyItem();
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
        Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity).PlayBurstEffectAndForget();
        ItemDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}