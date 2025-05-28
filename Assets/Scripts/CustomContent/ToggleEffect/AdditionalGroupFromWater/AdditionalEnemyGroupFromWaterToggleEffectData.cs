using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AdditionalEnemyGroupFromWaterToggleEffectData", menuName = "Effects/AdditionalEnemyGroupFromWaterToggleEffectData")]

public sealed class AdditionalEnemyGroupFromWaterToggleEffectData : ToggleGlobalEffectData
{
    [SerializeField] private AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup _enemyGroup;
    public override Type EffectInstanceType => typeof(AdditionalEnemyGroupFromWaterToggleEffect);
    
    public override void Modify(GlobalToggleEffect effect)
    {
        AdditionalEnemyGroupFromWaterToggleEffect additionalGroupToggleEffect = (AdditionalEnemyGroupFromWaterToggleEffect)effect;
        
        additionalGroupToggleEffect.SetAdditionalEnemyGroup(_enemyGroup);
    }
    
    private void OnValidate()
    {
        AllEffectTypeNames = new List<string>{"AdditionalEnemyGroupFromWaterToggleEffect"};
    }
}