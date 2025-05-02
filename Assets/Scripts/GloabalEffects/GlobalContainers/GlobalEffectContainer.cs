using System.Collections.Generic;
using Zenject;

public sealed class GlobalEffectContainer
{
    [Inject] private DiContainer _container;
    [Inject] private GlobalEffectFactory _globalEffectFactory;
    
    private ListDictionary<ToggleGlobalEffectData, GlobalToggleEffect> _activeToggleEffects = new();

    public void AddEffects(List<ToggleGlobalEffectData> effectDatas) => effectDatas.ForEach(data => AddEffect(data));
    
    public void AddEffect(ToggleGlobalEffectData globalEffectData)
    {
        GlobalToggleEffect effect = _globalEffectFactory.CreateToggleEffect(globalEffectData);
        
        _activeToggleEffects.Add(globalEffectData, effect);
        
        effect.Enable();
    }
    
    public void RemoveEffects(List<ToggleGlobalEffectData> effectDatas) => effectDatas.ForEach(data => RemoveEffect(data));

    public void RemoveEffect(ToggleGlobalEffectData globalEffectData)
    {
        if (_activeToggleEffects.Contains(globalEffectData))
        {
            GlobalToggleEffect toggleEffect = _activeToggleEffects.Remove(globalEffectData);
            
            toggleEffect.Disable();
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {globalEffectData.EffectType} but it doesn't exist");
    }
}