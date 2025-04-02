using System.Collections.Generic;
using UnityEngine;
using Cashing;
using System;

namespace Combat
{
    public sealed class ParticleEffectManager : MonoBehaviour
    {
        [Cached] private EntityEffectManager _entityEffectManager;
        [Cached] private CombatEntity _onwerEntity;
        
        private Dictionary<Type, EntityEffectParticleHandler> _particleHandlers = new();
        
        private void Start()
        {
            _entityEffectManager.EffectApplied += ApplyEffect;
            _entityEffectManager.EffectRemoved += RemoveEffect;
            _entityEffectManager.EffectUpdated += UpdateEffect;
        }
    
        private void ApplyEffect(Type effectType)
        {
            if (_particleHandlers.ContainsKey(effectType)) Debug.Log($"Effect already applied: {effectType}");
                
            EntityEffectParticleHandler particleHandler = EntityEffectParticlePool.Instance.GetFreeParticleHandler(effectType);
            
            particleHandler.AdaptToEntity(_onwerEntity);
            particleHandler.UpdateEffectStrength(_entityEffectManager.GetEffectPercentStrength(effectType));
            
            _particleHandlers.Add(effectType, particleHandler);
        }

        private void UpdateEffect(Type effectType)
        {
            if (!_particleHandlers.ContainsKey(effectType)) Debug.Log($"Effect does not exsist: {effectType}");
            
            _particleHandlers[effectType].UpdateEffectStrength(_entityEffectManager.GetEffectPercentStrength(effectType));
        }
        
        private void RemoveEffect(Type effectType)
        {
            if (!_particleHandlers.ContainsKey(effectType)) Debug.Log($"Effect does not exsist: {effectType}");
            
            _particleHandlers[effectType].Remove();
            
            _particleHandlers.Remove(effectType);
        }
    }
}