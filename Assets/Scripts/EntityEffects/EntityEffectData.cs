using UnityEngine;

[CreateAssetMenu(fileName = "EntityEffectData", menuName = "EntityEffects/EntityEffectData")]

public sealed class EntityEffectData : ScriptableObject
{
    [SerializeField] private int _maxStacks = 1;
    [SerializeField] private ArgumentsContainer _argumentsContainer;
    [SerializeField] private EntityEffectParticleHandler _particlePrefab;
    
    public int MaxStacks => _maxStacks;
    public ArgumentsContainer ArgumentsContainer => _argumentsContainer;
    public EntityEffectParticleHandler ParticlePrefab => _particlePrefab;
}