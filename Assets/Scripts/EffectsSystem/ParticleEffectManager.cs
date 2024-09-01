using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.VFX;

public sealed class ParticleEffectManager : MonoBehaviour
{
    private EntityEffectManager _entityEffectManager;

    private Dictionary<Effect, VisualEffectPoolObjectHandler> _appliedEffects = new Dictionary<Effect, VisualEffectPoolObjectHandler>();

    private void Awake()
    {
        _entityEffectManager = GetComponent<EntityEffectManager>();
    
        _entityEffectManager.EffectApplied.AddListener(ApplyEffect);
        _entityEffectManager.EffectRemoved.AddListener(RemoveEffect);
    }

    private void ApplyEffect(Effect effect)
    {
        if (_appliedEffects.ContainsKey(effect) == false)
        {        
            VisualEffectPoolObjectHandler visualEffect = effect.GetEffectObject();
    
            _appliedEffects.Add(effect, visualEffect);

            visualEffect.transform.SetParent(transform);

            visualEffect.transform.localPosition = new Vector3(0f, effect.InstantiationHeight, 0f);

            visualEffect.transform.localRotation = Quaternion.identity;

            visualEffect.transform.localScale = Vector3.one;
        }
    }

    private void RemoveEffect(Effect effect)
    {
        if (_entityEffectManager.HasEffect(effect) == false)
        {   
            if (_appliedEffects.ContainsKey(effect))
            {
                VisualEffectPoolObjectHandler visualEffect = _appliedEffects[effect];

                _appliedEffects.Remove(effect);

                visualEffect.transform.SetParent(null);

                visualEffect.Stop();
            }
        }
    }
}
