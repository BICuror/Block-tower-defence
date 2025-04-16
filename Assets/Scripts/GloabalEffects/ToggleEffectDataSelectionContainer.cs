using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToggleEffectDataSelectionContainer", menuName = "Effects/ToggleEffectDataSelectionContainer")]

public sealed class ToggleEffectDataSelectionContainer : ScriptableObject
{
    [SerializeField] private List<ToggleGlobalEffectData> _effects;
    
    public List<ToggleGlobalEffectData> GetGlobalEffects(int count)
    {
        List<ToggleGlobalEffectData> effects = new List<ToggleGlobalEffectData>(_effects);
        List<ToggleGlobalEffectData> resultEffects = new();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, effects.Count);
            
            resultEffects.Add(effects[randomIndex]);
            effects.RemoveAt(randomIndex);
        }
        
        return resultEffects;
    }
}