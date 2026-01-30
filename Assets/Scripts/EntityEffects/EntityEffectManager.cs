using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cashing;
using System;

namespace Combat
{
    public class EntityEffectManager : MonoBehaviour
    {
        [Cached] private DraggableObject _draggableObject;
        [Cached] private CombatEntity _ownerEntity;
        [Cached] private EntityHealth _entityHealth;
        private Dictionary<Type, EntityEffect> _appliedEffects = new();
        private Dictionary<Type, EntityEffectRemovalHandler> _removalHandlers = new();
        
        public List<Type> AppliedEffectTypes => _appliedEffects.Keys.ToList();
        public Dictionary<Type, EntityEffect> AppliedEffects => _appliedEffects;
        
        private bool EffectsCanBeApplied => _draggableObject.IsPlaced && _entityHealth.IsAlive();
        
        public Action<Type> EffectApplied;
        public Action<Type> EffectUpdated;
        public Action<Type> EffectRemoved;
        
        private void Start()
        {
            DraggableObject draggableObject = _ownerEntity.ComponentsContainer.Get<DraggableObject>();
            
            _ownerEntity.Health.Died += RemoveAllEffects;
            draggableObject.PickedUp += RemoveAllEffects;
        }
        
        public bool HasEffect(EntityEffectType effectType) => _appliedEffects.Values.ToList().Exists(effect => effect.EffectType == effectType);
        
        public bool HasEffect(Type effectType) => _appliedEffects.ContainsKey(effectType);
        
        public float GetEffectPercentStrength(Type effect) => (float)_appliedEffects[effect].Stack / (float)_appliedEffects[effect].MaxStacks;

        public void TryApplyTemporaryEffect(Type effectType, int strength, float duration)
        {
            if (!EffectsCanBeApplied) return;

            if (_appliedEffects.TryGetValue(effectType, out EntityEffect exsistingEffect))
            {
                UpdateEffect(exsistingEffect, effectType, strength);
                
                if (_removalHandlers.TryGetValue(effectType, out EntityEffectRemovalHandler removalHandler))
                { 
                    removalHandler.SetRemovalTimer(duration);
                    removalHandler.AddStacks(strength);
                }
                else
                {
                    AddRemovalHandler(effectType, strength, duration);
                }
            }
            else if (ApplyEffect(effectType, strength))
            {
                AddRemovalHandler(effectType, strength, duration);
            }
        }

        private void AddRemovalHandler(Type effectType, int strength, float duration)
        {
            EntityEffectRemovalHandler removalHandler = new(effectType);
            removalHandler.SetRemovalTimer(duration);
            removalHandler.AddStacks(strength);
            removalHandler.EffectRemovalTimerFinished += RemoveEffect;
            _removalHandlers.Add(effectType, removalHandler);
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

            if (!effect.CanBeApplied()) return false;
            
            _appliedEffects.Add(effectType, effect); 
            effect.SetStack(strength);
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
        
        private void RemoveAllEffects()
        {
            _appliedEffects.Keys.ToList().ForEach(RemoveEffect);
        }

        private void RemoveEffect(Type effectType) => RemoveEffect(effectType, int.MaxValue);
        
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
                        _removalHandlers[effectType].EffectRemovalTimerFinished -= RemoveEffect;
                        _removalHandlers[effectType].StopRemovalTimer();
                        _removalHandlers.Remove(effectType);
                    }
                                
                    EffectRemoved?.Invoke(effectType);
                }
            }
        }

        private void OnDestroy()
        {
            _ownerEntity.Health.Died -= RemoveAllEffects;
            _draggableObject.PickedUp -= RemoveAllEffects;
        }
    }
}