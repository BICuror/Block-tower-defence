using UnityEngine;
using System;
using Combat;

public sealed class EntityEffectFactory : MonoBehaviour
{
    [SerializeField] private EntityEffectDataContainer _effectDataContainer;
    private static EntityEffectFactory _instance;
    
    public static EntityEffectFactory Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Multiple EntityEffectFactory instances found");
            Destroy(this);
        }

        _instance = this;
    }
    
    public EntityEffect CreateEntityEffect(Type effectType)
    {
        EntityEffect effect = Activator.CreateInstance(effectType) as EntityEffect;
        EntityEffectConfig effectConfig = _effectDataContainer.GetEffectData(effectType);
        effect.Initialize(effectConfig.ArgumentsContainer, effectConfig);
        return effect;
    }
}