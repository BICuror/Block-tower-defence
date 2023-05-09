using UnityEngine;

public abstract class RemoveOverTimeEffect : Effect
{
    [SerializeField] private float _effectDuration;
    public float EffecDuration {get => _effectDuration;}
    
    public override EffectType GetEffectType() => EffectType.RemoveOverTime;
}
