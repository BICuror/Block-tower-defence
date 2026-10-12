using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityEffectData", menuName = "EntityEffects/EntityEffectData")]

public sealed class EntityEffectData : ScriptableObject
{
    [SerializeField] private int _maxStacks = 1;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    [SerializeField] private EntityEffectParticleHandler _particlePrefab;
    [SerializeField] private string _effectType;
    
    public int MaxStacks => _maxStacks;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EntityEffectParticleHandler ParticlePrefab => _particlePrefab;
    
    public Type GetEffectType() => Type.GetType(_effectType);
}