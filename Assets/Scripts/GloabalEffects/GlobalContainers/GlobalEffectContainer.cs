using System.Collections.Generic;
using Zenject;

public sealed class GlobalEffectContainer
{
    [Inject] private DiContainer _container;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    private ListDictionary<GlobalEffectData, List<GlobalEffect>> _activeEffects = new();

    public void AddEffects(List<GlobalEffectData> effectDatas) => effectDatas.ForEach(AddEffect);
    
    public void AddEffect(GlobalEffectData globalEffectData)
    {
        List<GlobalEffect> effects = _globalEffectFactory.CreateEffects(globalEffectData);
        
        _activeEffects.Add(globalEffectData, effects);
        
        effects.ForEach(effect => effect.Enable());
    }
    
    public void RemoveEffects(List<GlobalEffectData> effectDatas) => effectDatas.ForEach(RemoveEffect);

    public void RemoveEffect(GlobalEffectData globalEffectData)
    {
        if (_activeEffects.Contains(globalEffectData))
        {
            List<GlobalEffect> effects = _activeEffects.Remove(globalEffectData);
            
            effects.ForEach(effect => effect.Disable());
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {globalEffectData.EffectType} but it doesn't exist");
    }
}