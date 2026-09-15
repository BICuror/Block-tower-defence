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
    
    [Header("Scale")]
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minScale;
    
    private List<GlobalEffectData> _effectDatas = new();
    private int _charges;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private GlobalEffectData _toggleGlobalEffectData;
    
    [Button]
    public void ApplyEffect()
    {
        if (_effectDatas.Contains(_toggleGlobalEffectData)) return;
        
        _effectDatas.Add(_toggleGlobalEffectData);
    }
    
#endif 
    public List<GlobalEffectData> EffectDatas => _effectDatas;
    public int Charges => _charges;
    public ItemColor ItemColor => _itemColor;
    
    public event Action<Item> ItemPickedUp;
    public event Action<Item> ItemDestroyed;
    
    private void Awake()
    {
        base.Awake();
        PickedUp += () => ItemPickedUp?.Invoke(this);
    }

    public void AddToggleEffectDatas(List<GlobalEffectData> effectDatas) => _effectDatas.AddRange(effectDatas); 

    public void SetChargesAmount(int charges)
    {
        _charges = charges;
        
        float scale = Mathf.Lerp(_maxScale, _minScale, charges / 5.5f);
        
        AnimationObject.transform.localScale = new Vector3(scale, scale, scale);
    } 

    public async UniTask DecreaseDuration()
    {
        if (_charges == 0) await UniTask.WaitForSeconds(1f);
        
        await _upgradeChargeContainer.AddChargesWithAnimation(_charges, transform);
        
        DestroyItem();
    }
    
    public void EnableToggleEffects() => _globalEffectContainer.AddEffects(_effectDatas);
    
    public void DisableToggleEffects() => _globalEffectContainer.RemoveEffects(_effectDatas);
    
    public void DestroyItem()
    {
        Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity).PlayBurstEffectAndForget();
        ItemDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}