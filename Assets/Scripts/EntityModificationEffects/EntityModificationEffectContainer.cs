using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityModificationEffectContainer : MonoBehaviour
{
    private readonly Dictionary<EntityModificatorData, List<EntityModificatior>> _activeEffects = new();
    [SerializeField] private List<EntityModificatorData> _allAvailableEffects;
    [Inject] private EffectFactory _effectFactory;
    [Cached] private CombatEntity _ownerEntity;
    
    public List<EntityModificatorData> AvailableEffects => new List<EntityModificatorData>(_allAvailableEffects);
    public List<EntityModificatorData> ActiveEffect => _activeEffects.Keys.ToList();
    
    public void AddEffect(EntityModificatorData modificatorData)
    {
        EntityModificatior modificatior = _effectFactory.CreateEntityModificationEffect(modificatorData);
        modificatior.SetEntity(_ownerEntity);
        
        modificatior.Enable();

        if (_activeEffects.TryGetValue(modificatorData, out var effectList))
        {
            effectList.Add(modificatior);
        }
        else
        {
            _activeEffects[modificatorData] = new() {modificatior};
        }
    }
    
    public void RemoveEffect(EntityModificatorData modificatorData)
    {
        if (_activeEffects.TryGetValue(modificatorData, out var effectList))
        {
            EntityModificatior modificatior = effectList[^1];
            modificatior.Disable();
            _activeEffects[modificatorData].Remove(modificatior);
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {modificatorData.EffectType} but it doesn't exist");
    }
}