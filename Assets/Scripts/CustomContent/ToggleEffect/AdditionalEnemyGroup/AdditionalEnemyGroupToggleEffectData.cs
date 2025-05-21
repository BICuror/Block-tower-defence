using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AdditionalEnemyGroupToggleEffectData", menuName = "Effects/AdditionalEnemyGroupToggleEffectData")]

public sealed class AdditionalEnemyGroupToggleEffectData : ToggleGlobalEffectData
{
    [SerializeField] private AdditionalEnemyGroup _enemyGroup;
    public override Type EffectInstanceType => typeof(AdditionalEnemyGroupToggleEffect);
    
    public override void Modify(GlobalToggleEffect effect)
    {
        AdditionalEnemyGroupToggleEffect additionalGroupToggleEffect = (AdditionalEnemyGroupToggleEffect)effect;
        
        additionalGroupToggleEffect.SetAdditionalEnemyGroup(_enemyGroup);
    }
    
    private void OnValidate()
    {
        AllEffectTypeNames = new List<string>{"AdditionalEnemyGroupToggleEffect"};
    }

    [Serializable] public sealed class AdditionalEnemyGroup
    {
        [SerializeField] private float _groupHealth;
        [SerializeField] private List<EnemyWaveGroup.GroupPart> _groupParts;
        
        public float GroupHealth => _groupHealth;
        public List<EnemyWaveGroup.GroupPart> GroupParts => _groupParts;
    }
}