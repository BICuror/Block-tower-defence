using System.Collections.Generic;
using Zenject;

public sealed class GlobalEffectContainer
{
    [Inject] private DiContainer _container;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    private ListDictionary<ToggleGlobalEffectData, List<GlobalToggleEffect>> _activeToggleEffects = new();

    public void AddEffects(List<ToggleGlobalEffectData> effectDatas) => effectDatas.ForEach(data => AddEffect(data));
    
    public void AddEffect(ToggleGlobalEffectData globalEffectData)
    {
        List<GlobalToggleEffect> effects = _globalEffectFactory.CreateToggleEffects(globalEffectData);
        
        _activeToggleEffects.Add(globalEffectData, effects);
        
        effects.ForEach(effect => effect.Enable());
    }
    
    public void RemoveEffects(List<ToggleGlobalEffectData> effectDatas) => effectDatas.ForEach(data => RemoveEffect(data));

    public void RemoveEffect(ToggleGlobalEffectData globalEffectData)
    {
        if (_activeToggleEffects.Contains(globalEffectData))
        {
            List<GlobalToggleEffect> effects = _activeToggleEffects.Remove(globalEffectData);
            
            effects.ForEach(effect => effect.Disable());
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {globalEffectData.EffectType} but it doesn't exist");
    }
}