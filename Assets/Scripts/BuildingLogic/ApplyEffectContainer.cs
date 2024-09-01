using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class ApplyEffectContainer : MonoBehaviour
{
    [SerializeField] private List<Effect> _applyEffects = new List<Effect>(); 

    public UnityEvent ApplyEffectUpdated;

    public List<Effect> GetApplyEffects() => _applyEffects;

    public int GetAmountOfEffects() => _applyEffects.Count;

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
