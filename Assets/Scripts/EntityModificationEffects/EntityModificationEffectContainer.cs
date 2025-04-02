using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityModificationEffectContainer : MonoBehaviour
{
    private readonly Dictionary<EntityModificationEffectData, List<EntityModificationEffect>> _activeEffects = new();
    [SerializeField] private List<EntityModificationEffectData> _allAvailableEffects;
    [Inject] private EffectFactory _effectFactory;
    [Cached] private CombatEntity _ownerEntity;
    
    public List<EntityModificationEffectData> AvailableEffects => new List<EntityModificationEffectData>(_allAvailableEffects);
    
    public void AddEffect(EntityModificationEffectData modificationEffectData)
    {
        EntityModificationEffect modificationEffect = _effectFactory.CreateEntityModificationEffect(modificationEffectData);
        modificationEffect.SetEntity(_ownerEntity);
        
        modificationEffect.Enable();

        if (_activeEffects.TryGetValue(modificationEffectData, out var effectList))
        {
            effectList.Add(modificationEffect);
        }
        else
        {
            _activeEffects[modificationEffectData] = new() {modificationEffect};
        }
    }
    
    public void RemoveEffect(EntityModificationEffectData modificationEffectData)
    {
        if (_activeEffects.TryGetValue(modificationEffectData, out var effectList))
        {
            EntityModificationEffect modificationEffect = effectList[^1];
            modificationEffect.Disable();
            _activeEffects[modificationEffectData].Remove(modificationEffect);
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {modificationEffectData.EffectType} but it doesn't exist");
    }
}