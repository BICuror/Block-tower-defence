using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class GlobalEffectContainer : MonoBehaviour
{
    [Inject] private DiContainer _container;
    [Inject] private EffectFactory _effectFactory;
    
    private Dictionary<ToggleEffectData, List<ToggleEffect>> _activeToggleEffects = new();

    public void AddEffects(List<ToggleEffectData> effectDatas) => effectDatas.ForEach(data => AddEffect(data));
    
    public void AddEffect(ToggleEffectData effectData)
    {
        ToggleEffect effect = _effectFactory.CreateToggleEffect(effectData);
        effect.Enable();

        if (_activeToggleEffects.TryGetValue(effectData, out var effectList))
        {
            effectList.Add(effect);
        }
        else
        {
            _activeToggleEffects[effectData] = new() {effect};
        }
    }
    
    public void RemoveEffects(List<ToggleEffectData> effectDatas) => effectDatas.ForEach(data => RemoveEffect(data));

    public void RemoveEffect(ToggleEffectData effectData)
    {
        if (_activeToggleEffects.TryGetValue(effectData, out var effectList))
        {
            ToggleEffect effect = effectList[^1];
            effect.Disable();
            _activeToggleEffects[effectData].Remove(effect);
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {effectData.EffectType} but it doesn't exist");
    }
}