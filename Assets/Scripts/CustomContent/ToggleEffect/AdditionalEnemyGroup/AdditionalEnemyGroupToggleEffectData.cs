using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AdditionalEnemyGroupToggleEffectData", menuName = "Effects/AdditionalEnemyGroupToggleEffectData")]

public sealed class AdditionalEnemyGroupToggleEffectData : ToggleGlobalEffectData
{
    [SerializeField] private AdditionalEnemyGroup _enemyGroup;
    
    public override void Modify(GlobalEffect effect)
    {
        if (effect is not AdditionalEnemyGroupToggleEffect) return;
        
        AdditionalEnemyGroupToggleEffect additionalGroupToggleEffect = (AdditionalEnemyGroupToggleEffect)effect;
        
        additionalGroupToggleEffect.SetAdditionalEnemyGroup(_enemyGroup);
    }
    
    [Serializable] public sealed class AdditionalEnemyGroup
    {
        [Range(0f, 1f)] [SerializeField] private float _amountMultiplier;
        [SerializeField] private List<EnemyWaveGroup.GroupPart> _groupParts;
    
        public float AmountMultiplier => _amountMultiplier;
        public List<EnemyWaveGroup.GroupPart> GroupParts => _groupParts;
    }
}