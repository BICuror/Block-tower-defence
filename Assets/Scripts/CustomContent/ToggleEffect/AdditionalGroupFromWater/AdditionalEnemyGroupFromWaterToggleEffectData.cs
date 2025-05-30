using UnityEngine;

[CreateAssetMenu(fileName = "AdditionalEnemyGroupFromWaterToggleEffectData", menuName = "Effects/AdditionalEnemyGroupFromWaterToggleEffectData")]

public sealed class AdditionalEnemyGroupFromWaterToggleEffectData : ToggleGlobalEffectData
{
    [SerializeField] private AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup _enemyGroup;
    
    public override void Modify(GlobalEffect effect)
    {
        if (effect is not AdditionalEnemyGroupFromWaterToggleEffect) return;
        
        AdditionalEnemyGroupFromWaterToggleEffect additionalGroupToggleEffect = (AdditionalEnemyGroupFromWaterToggleEffect)effect;
        
        additionalGroupToggleEffect.SetAdditionalEnemyGroup(_enemyGroup);
    }
}