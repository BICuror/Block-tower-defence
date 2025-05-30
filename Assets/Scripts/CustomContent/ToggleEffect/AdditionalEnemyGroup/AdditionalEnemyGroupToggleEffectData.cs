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
        [SerializeField] private float _groupHealth;
        [SerializeField] private List<EnemyWaveGroup.GroupPart> _groupParts;
        
        public float GroupHealth => _groupHealth;
        public List<EnemyWaveGroup.GroupPart> GroupParts => _groupParts;
    }
}