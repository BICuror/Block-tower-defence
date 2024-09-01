using UnityEngine;
using System.Collections.Generic;

public sealed class EffectInitialisationChecker : MonoBehaviour
{
    private static EffectInitialisationChecker _instance;
    public static EffectInitialisationChecker Instance => _instance;

    private List<Effect> _initializedEffects = new List<Effect>();

    public bool EffectIsInitialized(Effect effect)
    {
        if (_initializedEffects.Contains(effect))
        {
            return true;
        }
        else
        {
            _initializedEffects.Add(effect);

            return false;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of effectInitialisationChecker");
        }
    }    
}
