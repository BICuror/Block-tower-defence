using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public abstract class Effect : ScriptableObject 
{
    [Header("EffectVisualSettings")]
    [SerializeField] private VisualEffectPoolObjectHandler _effectPrefab;
    [SerializeField] private float _instantiationHeight;
    public float InstantiationHeight => _instantiationHeight;

    [SerializeField] private int _initialEffectPoolSize;

    protected ObjectPool<VisualEffectPoolObjectHandler> _visualEffectsPool; 

    public void InstaitiateVisualEffectsPool()
    {
        _visualEffectsPool = new ObjectPool<VisualEffectPoolObjectHandler>(_effectPrefab, _initialEffectPoolSize);
    }   

    public VisualEffectPoolObjectHandler GetEffectObject() 
    {
        if (EffectInitialisationChecker.Instance.EffectIsInitialized(this) == false) InstaitiateVisualEffectsPool();

        return _visualEffectsPool.GetNextPooledObject();
    }
    public abstract EffectType GetEffectType();

    public abstract bool CanBeApplied(EntityComponentsContainer componentsContainer);
    
    public abstract void ApplyToEntity(EntityComponentsContainer componentsContainer);

    public abstract void RemoveFromEntity(EntityComponentsContainer componentsContainer);  
}

public enum EffectType
{
    Permanent,
    PermanentOverTime,
    RemoveOverTime,
    RemoveOverTicks
}
