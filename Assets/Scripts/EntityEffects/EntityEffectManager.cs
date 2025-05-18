using System.Collections.Generic;
using UnityEngine;
using Cashing;
using System;
using System.Linq;

namespace Combat
{
    public class EntityEffectManager : MonoBehaviour
    {
        [Cached] private DraggableObject _draggableObject;
        [Cached] private CombatEntity _ownerEntity;
        private Dictionary<Type, EntityEffect> _appliedEffects = new();
        private Dictionary<Type, EntityEffectRemovalHandler> _removalHandlers = new();
        private bool _effectsCanBeSet = true;
        
        private bool EffectsCanBeApplied => _draggableObject.IsPlaced;

        public Action<Type> EffectApplied;
        public Action<Type> EffectUpdated;
        public Action<Type> EffectRemoved;
        
        private void Start()
        {
            DraggableObject draggableObject = _ownerEntity.ComponentsContainer.Get<DraggableObject>();
            
            _ownerEntity.ComponentsContainer.Get<EntityHealth>().EntityDied += _ => RemoveAllEffects();
            draggableObject.PickedUp += RemoveAllEffects;
        }
        
        public bool HasEffect(Type effectType) => _appliedEffects.ContainsKey(effectType);
        
        public float GetEffectPercentStrength(Type effect) => (float)_appliedEffects[effect].Stack / (float)_appliedEffects[effect].MaxStacks;

        public void TryApplyTemporaryEffect(Type effectType, int strength, float duration)
        {
            if (!EffectsCanBeApplied) return;

            if (_appliedEffects.TryGetValue(effectType, out EntityEffect exsistingEffect))
            {
                UpdateEffect(exsistingEffect, effectType, strength);
                
                _removalHandlers[effectType].UpdateRemovalTimer(duration);
            }
            else
            {
                if (ApplyEffect(effectType, strength))
                {
                    EntityEffectRemovalHandler removalHandler = new(effectType);
                    removalHandler.SetRemovalTimer(duration);
                    removalHandler.EffectRemovalTimerFinished += RemoveEffect;
                    _removalHandlers.Add(effectType, removalHandler);
                }
            }
        }
        
        public void TryApplyEffect(Type effectType, int strength)
        {
            if (!EffectsCanBeApplied) return;
            
            if (_appliedEffects.TryGetValue(effectType, out EntityEffect exsistingEffect))
            {
                UpdateEffect(exsistingEffect, effectType, strength);
            }
            else
            {
                ApplyEffect(effectType, strength);
            }
        }

        private bool ApplyEffect(Type effectType, int strength)
        {
            EntityEffect effect = EntityEffectFactory.Instance.CreateEntityEffect(effectType);
            effect.SetEntity(_ownerEntity); 
            effect.SetStack(strength);

            if (!effect.CanBeApplied()) return false;
            
            _appliedEffects.Add(effectType, effect); 
            effect.ApplyToEntity();
            
            EffectApplied?.Invoke(effectType);
            
            return true;
        }

        private void UpdateEffect(EntityEffect exsistingEffect, Type effectType, int strength)
        {
            exsistingEffect.ChangeStack(strength);
            exsistingEffect.Update();
                
            EffectUpdated?.Invoke(effectType);
        }
        
        public void RemoveAllEffects()
        {
            List<Type> effectTypes = _appliedEffects.Keys.ToList();
            
            while (_appliedEffects.Count > 0)
            {
                RemoveEffect(effectTypes[^1]);
            }
        }

        public void RemoveEffect(Type effectType) => RemoveEffect(effectType, int.MaxValue);
        
        public void RemoveEffect(Type effectType, int strength)
        {
            if (_appliedEffects.TryGetValue(effectType, out EntityEffect exsistingEffect))
            {
                exsistingEffect.ChangeStack(-strength);
                            
                if (exsistingEffect.TrueStack > 0)
                {
                    exsistingEffect.Update();
                                
                    EffectUpdated?.Invoke(effectType);
                }
                else
                {
                    exsistingEffect.RemoveFromEntity();
                    _appliedEffects.Remove(effectType);
                                
                    if (_removalHandlers.ContainsKey(effectType))
                    {
                        _removalHandlers[effectType].StopRemovalTimer();
                        _removalHandlers[effectType].EffectRemovalTimerFinished -= RemoveEffect;
                        _removalHandlers.Remove(effectType);
                    }
                                
                    EffectRemoved?.Invoke(effectType);
                }
            }
        }
    }
}