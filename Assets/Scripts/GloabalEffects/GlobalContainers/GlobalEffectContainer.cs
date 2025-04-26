using System.Collections.Generic;
using Zenject;

public sealed class GlobalEffectContainer
{
    [Inject] private DiContainer _container;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    private Dictionary<ToggleGlobalEffectData, List<GlobalToggleEffect>> _activeToggleEffects = new();

    public void AddEffects(List<ToggleGlobalEffectData> effectDatas) => effectDatas.ForEach(data => AddEffect(data));
    
    public void AddEffect(ToggleGlobalEffectData globalEffectData)
    {
        GlobalToggleEffect effect = _globalEffectFactory.CreateToggleEffect(globalEffectData);
        effect.Enable();

        if (_activeToggleEffects.TryGetValue(globalEffectData, out var effectList))
        {
            effectList.Add(effect);
        }
        else
        {
            _activeToggleEffects[globalEffectData] = new() {effect};
        }
    }
    
    public void RemoveEffects(List<ToggleGlobalEffectData> effectDatas) => effectDatas.ForEach(data => RemoveEffect(data));

    public void RemoveEffect(ToggleGlobalEffectData globalEffectData)
    {
        if (_activeToggleEffects.TryGetValue(globalEffectData, out var effectList))
        {
            GlobalToggleEffect effect = effectList[^1];
            effect.Disable();
            _activeToggleEffects[globalEffectData].Remove(effect);
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {globalEffectData.EffectType} but it doesn't exist");
    }
}