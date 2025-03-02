using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityEffectContainer : MonoBehaviour
{
    private readonly Dictionary<EntityEffectData, List<EntityEffect>> _activeEffects = new();
    [SerializeField] private List<EntityEffectData> _allAvailableEffects;
    [Inject] private EffectFactory _effectFactory;
    [Cached] private CombatEntity _ownerEntity;
    
    public List<EntityEffectData> AvailableEffects => new List<EntityEffectData>(_allAvailableEffects);
    
    public void AddEffect(EntityEffectData effectData)
    {
        EntityEffect effect = _effectFactory.CreateEntityEffect(effectData);
        effect.SetEntity(_ownerEntity);
        
        effect.Enable();

        if (_activeEffects.TryGetValue(effectData, out var effectList))
        {
            effectList.Add(effect);
        }
        else
        {
            _activeEffects[effectData] = new() {effect};
        }
    }
    
    public void RemoveEffect(EntityEffectData effectData)
    {
        if (_activeEffects.TryGetValue(effectData, out var effectList))
        {
            EntityEffect effect = effectList[^1];
            effect.Disable();
            _activeEffects[effectData].Remove(effect);
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {effectData.EffectType} but it doesn't exist");
    }
}