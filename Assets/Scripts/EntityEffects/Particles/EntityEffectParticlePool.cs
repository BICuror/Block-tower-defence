using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class EntityEffectParticlePool : MonoBehaviour
{
    [SerializeField] private EntityEffectDataContainer _entityEffectDataContainer;
    private Dictionary<Type, ObjectPool<EntityEffectParticleHandler>> _particlePools = new();
    private static EntityEffectParticlePool _instance;
    
    public static EntityEffectParticlePool Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Multiple EntityEffectParticlePool instances found");
            Destroy(this);
        }

        _instance = this;
    }

    public EntityEffectParticleHandler GetFreeParticleHandler(Type effectType)
    {
        if (!_particlePools.ContainsKey(effectType))
        {
            EntityEffectParticleHandler particleHandlerPrefab = _entityEffectDataContainer.GetEffectData(effectType).ParticlePrefab;

            ObjectPool<EntityEffectParticleHandler> newPool = new ObjectPool<EntityEffectParticleHandler>(particleHandlerPrefab, 3);

            _particlePools.Add(effectType, newPool);
        }

        return _particlePools[effectType].GetNextPooledObject();
    }
}