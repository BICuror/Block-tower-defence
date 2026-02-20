using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GlobalStatChangeToggleEffectData", menuName = "Effects/GlobalStatChangeToggleEffectData")]

public sealed class GlobalStatChangeToggleEffectData : GlobalEffectData
{ 
    [SerializeField] private List<StatChange> _statChanges;
    
    public override void Modify(GlobalEffect effect)
    {
        if (effect is not GlobalStatChangeToggleEffect) return;
        
        GlobalStatChangeToggleEffect statChangeEffect = (GlobalStatChangeToggleEffect)effect;
        
        statChangeEffect.SetStatChanges(_statChanges);
    }
}