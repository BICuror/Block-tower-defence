using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EntityEffectManager : MonoBehaviour
{
    private EntityComponentsContainer _entityComponentsContainer;

    private List<Effect> _appliedEffects;

    public UnityEvent<Effect> EffectApplied;

    public UnityEvent<Effect> EffectRemoved;

    private void Start()
    {
        _appliedEffects = new List<Effect>();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        EntityHealth health = GetComponent<EntityHealth>();
        TaskCycle taskCycle = GetComponent<TaskCycle>();

        _entityComponentsContainer = new EntityComponentsContainer(health, taskCycle, agent);
    }

    #region EffectOverTicks
    public void ApplyEffectOverTicks(RemoveOverTicksEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);
        
        StartCoroutine(WaitToApplyEffectOverTime(effect, effect.TicksAmount));   
    }

    private IEnumerator WaitToApplyEffectOverTime(RemoveOverTicksEffect effect, int iterations)
    {
        yield return new WaitForSeconds(effect.TickDuration);

        iterations--;

        effect.ApplyTickEffectToEntity(_entityComponentsContainer);
        
        if (iterations > 0) StartCoroutine(WaitToApplyEffectOverTime(effect, iterations));
        else RemoveEffect(effect);
    }    
    #endregion
    
    #region PermanentEffectOverTime
    public void ApplyEffectOverTimePermanently(PermanentOverTimeEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);
        
        StartCoroutine(WaitToPermemantlyApplyEffectOverTime(effect));   
    }

    private IEnumerator WaitToPermemantlyApplyEffectOverTime(PermanentOverTimeEffect effect)
    {
        yield return new WaitForSeconds(effect.TickDuration);

        effect.ApplyTickEffectToEntity(_entityComponentsContainer);
        
        StartCoroutine(WaitToPermemantlyApplyEffectOverTime(effect));
    }    
    #endregion

    #region ApplyAndRemoveOverTime
    public void ApplyEffectAndRemoveAfterTime(RemoveOverTimeEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);
        
        StartCoroutine(WaitToRemove(effect));    
    }

    private IEnumerator WaitToRemove(RemoveOverTimeEffect effect)
    {
        yield return new WaitForSeconds(effect.EffecDuration);
        
        RemoveEffect(effect);
    }
    #endregion

    #region ApplyEffectPermanently
    public void ApplyEffectPermanently(PermanentEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);
    }
    #endregion

    public void ApplyEffect(Effect effectToApply)
    {
        if (effectToApply.CanBeApplied(_entityComponentsContainer))
        {
            _appliedEffects.Add(effectToApply);

            EffectApplied?.Invoke(effectToApply);

            switch (effectToApply.GetEffectType())
            {
                case EffectType.Permanent: ApplyEffectPermanently(effectToApply as PermanentEffect); break;
                case EffectType.PermanentOverTime: ApplyEffectOverTimePermanently(effectToApply as PermanentOverTimeEffect); break;
                case EffectType.RemoveOverTime: ApplyEffectAndRemoveAfterTime(effectToApply as RemoveOverTimeEffect); break;
                case EffectType.RemoveOverTicks: ApplyEffectOverTicks(effectToApply as RemoveOverTicksEffect); break;
            }
        }
    }
    
    public void RemoveEffect(Effect effectToRemove)
    {
        if (HasEffect(effectToRemove))
        {
            _appliedEffects.Remove(effectToRemove);

            EffectRemoved?.Invoke(effectToRemove);

            effectToRemove.RemoveFromEntity(_entityComponentsContainer);
        }
    }

    public List<Effect> GetAllEffecs() => _appliedEffects;

    public bool HasEffect(Effect effect) => _appliedEffects.Contains(effect);
}
