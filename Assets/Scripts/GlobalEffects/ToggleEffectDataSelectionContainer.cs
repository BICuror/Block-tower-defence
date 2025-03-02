using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToggleEffectDataSelectionContainer", menuName = "Effects/ToggleEffectDataSelectionContainer")]

public sealed class ToggleEffectDataSelectionContainer : ScriptableObject
{
    [SerializeField] private List<ToggleEffectData> _effects;
    
    public List<ToggleEffectData> GetGlobalEffects(int count)
    {
        List<ToggleEffectData> effects = new List<ToggleEffectData>(_effects);
        List<ToggleEffectData> resultEffects = new();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, effects.Count);
            
            resultEffects.Add(effects[randomIndex]);
            effects.RemoveAt(randomIndex);
        }
        
        return resultEffects;
    }
}