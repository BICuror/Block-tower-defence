using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EntityEffectManager : MonoBehaviour
{
    [SerializeField] private List<Effect> _effectsImmunities;
    protected bool _effectsCanBeSet = true;
    private EntityComponentsContainer _entityComponentsContainer;

    private List<Effect> _appliedEffects = new List<Effect>();

    public UnityEvent<Effect> EffectApplied;

    public UnityEvent<Effect> EffectRemoved;
    
    private Dictionary<Effect, Coroutine> _coroutines = new Dictionary<Effect, Coroutine>();

    public List<Effect> GetAllEffecs() => _appliedEffects;

    public bool HasEffect(Effect effect) => _appliedEffects.Contains(effect);

    private void Start()
    {
        GetComponent<EntityHealth>().DeathEvent.AddListener(RemoveAllEffects);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        EntityHealth health = GetComponent<EntityHealth>();
        TaskCycle taskCycle = GetComponent<TaskCycle>();

        _entityComponentsContainer = new EntityComponentsContainer(health, taskCycle, agent);
    }

    private bool EffectCanBeApplied(Effect effect) => (_effectsCanBeSet && _effectsImmunities.Contains(effect) == false);
    
    public void ApplyEffect(Effect effectToApply)
    {
        if (EffectCanBeApplied(effectToApply) && effectToApply.CanBeApplied(_entityComponentsContainer) && gameObject.activeSelf)
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

    public void RemoveAllEffects(GameObject blank) => RemoveAllEffects();
    public void RemoveAllEffects()
    {
        StopAllCoroutines();

        while(_appliedEffects.Count > 0)
        {   
            RemoveEffect(_appliedEffects[_appliedEffects.Count - 1]);
        }   
    }

    public void TryToRemoveEffect(Effect effectToRemove)
    {
        if (_appliedEffects.Contains(effectToRemove))
        {
            RemoveEffect(effectToRemove);
        }
    }
    
    private void RemoveEffect(Effect effectToRemove)
    {
        _appliedEffects.Remove(effectToRemove);

        EffectRemoved?.Invoke(effectToRemove);

        if (_coroutines.ContainsKey(effectToRemove)) 
        {
            StopCoroutine(_coroutines[effectToRemove]);

            _coroutines.Remove(effectToRemove);
        }

        effectToRemove.RemoveFromEntity(_entityComponentsContainer);
    }

    #region EffectOverTicks
    public void ApplyEffectOverTicks(RemoveOverTicksEffect effect)
    {
        CheckToRemoveCoroutine(effect);

        _coroutines.Add(effect, StartCoroutine(ApplyEffectOverTime(effect)));   
    }

    private IEnumerator ApplyEffectOverTime(RemoveOverTicksEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);

        YieldInstruction instruction = new WaitForSeconds(effect.TickDuration);

        for (int i = 0; i < effect.TicksAmount; i++)
        {
            yield return instruction;

            effect.ApplyTickEffectToEntity(_entityComponentsContainer);
        }

        RemoveEffect(effect);
    }    
    #endregion
    
    #region PermanentEffectOverTime
    public void ApplyEffectOverTimePermanently(PermanentOverTimeEffect effect)
    {
        CheckToRemoveCoroutine(effect);
        
        _coroutines.Add(effect, StartCoroutine(PermemantlyApplyEffectOverTime(effect)));   
    }

    private IEnumerator PermemantlyApplyEffectOverTime(PermanentOverTimeEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);

        YieldInstruction instruction = new WaitForSeconds(effect.TickDuration);

        while (true)
        {
            yield return instruction;

            effect.ApplyTickEffectToEntity(_entityComponentsContainer);
        }
    }    
    #endregion

    #region ApplyAndRemoveOverTime
    public void ApplyEffectAndRemoveAfterTime(RemoveOverTimeEffect effect)
    {
        CheckToRemoveCoroutine(effect);

        _coroutines.Add(effect, StartCoroutine(ApplyAndRemove(effect)));   
    }

    private IEnumerator ApplyAndRemove(RemoveOverTimeEffect effect)
    {
        effect.ApplyToEntity(_entityComponentsContainer);

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

    private void CheckToRemoveCoroutine(Effect effect)
    {
        if (_coroutines.ContainsKey(effect))
        {
            if (_coroutines[effect] != null) StopCoroutine(_coroutines[effect]);  
            _coroutines.Remove(effect);  
            _appliedEffects.Remove(effect);
            effect.RemoveFromEntity(_entityComponentsContainer);
        }
    }
}