using UnityEngine;
using Configs;

namespace Combat.ValueModifiers
{
    public sealed class EntityEffectDifferentTypeValueModifier : ValueModifier
    {
        [Argument("EvaluateOtherEntityHealth")] private bool _evaluateOtherEntityHealth;
        [Argument("ExcludePositiveEffects")] private bool _excludePositiveEffects;
        [Argument("ExcludeNegativeEffects")] private bool _excludeNegativeEffects;
        [Argument("FlatChangePerStack")] private float _flatChange; 
        [Argument("MultChangePerStack")] private float _multChange;
        [Argument("MaxStacks")] private int _maxStacks;
        
        public override float GetFlatChange(CombatEntity otherEntity, float value) => _flatChange * GetCurrentStacksCount(otherEntity);
        public override float GetMultChange(CombatEntity otherEntity, float value) => _multChange * GetCurrentStacksCount(otherEntity);

        private int GetCurrentStacksCount(CombatEntity otherEntity)
        {
            int stacks = 0;
            
            CombatEntity evaluatedEntity = GetEvaluatedEntity(otherEntity);

            foreach (EntityEffect effect in evaluatedEntity.ComponentsContainer.Get<EntityEffectManager>().AppliedEffects.Values)
            {
                if (_excludeNegativeEffects && effect.EffectType == EntityEffectType.Negative) continue;
                
                if (_excludePositiveEffects && effect.EffectType == EntityEffectType.Positive) continue;

                stacks++;
            }
            
            return Mathf.Clamp(stacks, 0, _maxStacks);
        }

        private CombatEntity GetEvaluatedEntity(CombatEntity otherEntity)
        {
            if (_evaluateOtherEntityHealth) return otherEntity;
            
            return OwnerEntity;
        }
    }
}
