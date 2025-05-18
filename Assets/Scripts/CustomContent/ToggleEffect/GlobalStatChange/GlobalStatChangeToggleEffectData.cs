using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GlobalStatChangeToggleEffectData", menuName = "Effects/GlobalStatChangeToggleEffectData")]

public sealed class GlobalStatChangeToggleEffectData : ToggleGlobalEffectData
{ 
    [SerializeField] private List<StatChange> _statChanges;
    
    public override Type EffectInstanceType => typeof(GlobalStatChangeToggleEffect);
    
    public override void Modify(GlobalToggleEffect effect)
    {
        GlobalStatChangeToggleEffect statChangeEffect = (GlobalStatChangeToggleEffect)effect;
        
        statChangeEffect.SetStatChanges(_statChanges);
    }
    
    private void OnValidate()
    {
        AllEffectTypeNames = new List<string>{"GlobalStatChangeToggleEffect"};
    }
}