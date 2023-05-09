using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class ApplyEffectContainer : MonoBehaviour
{
    private List<Effect> _applyEffects = new List<Effect>(); 

    public UnityEvent ApplyEffectUpdated;

    public List<Effect> GetApplyEffects() => _applyEffects;

    public void RemoveEffect(Effect effectToRemove)
    {
        _applyEffects.Remove(effectToRemove);

        ApplyEffectUpdated.Invoke();
    }
    
    public void AddEffect(Effect effectToApply)
    {
        _applyEffects.Add(effectToApply);

        ApplyEffectUpdated.Invoke();
    }
}
