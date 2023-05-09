using UnityEngine;

public abstract class Effect : ScriptableObject 
{
    [SerializeField] private Material _uIMaterial;
    public Material UIMaterial {get => _uIMaterial;}

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
