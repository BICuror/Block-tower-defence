using UnityEngine;

public abstract class RemoveOverTicksEffect : Effect
{
    [SerializeField] private float _tickDuration;
    public float TickDuration {get => _tickDuration;}

    [SerializeField] private int _ticksAmount;
    public int TicksAmount {get => _ticksAmount;}
    
    public override EffectType GetEffectType() => EffectType.RemoveOverTicks;

    public abstract void ApplyTickEffectToEntity(EntityComponentsContainer componentsContainer);

    public override sealed void ApplyToEntity(EntityComponentsContainer componentsContainer) {}

    public override sealed void RemoveFromEntity(EntityComponentsContainer componentsContainer) {}
}
