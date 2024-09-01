using UnityEngine;

public abstract class PermanentOverTimeEffect : Effect
{
    [SerializeField] private float _tickDuration;
    public float TickDuration {get => _tickDuration;}

    public override EffectType GetEffectType() => EffectType.PermanentOverTime;

    public abstract void ApplyTickEffectToEntity(EntityComponentsContainer componentsContainer);
    
    public override sealed void ApplyToEntity(EntityComponentsContainer componentsContainer) {}

    public override sealed void RemoveFromEntity(EntityComponentsContainer componentsContainer) {}
}
