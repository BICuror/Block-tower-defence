using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityModificatorsContainer : MonoBehaviour
{
    private readonly Dictionary<EntityModificatorData, List<EntityModificatior>> _appliedModificators = new();
    [SerializeField] private List<EntityModificatorData> _allAvailableModificators;
    [Inject] private EntityModificatorFactory _entityModificatorFactory;
    [Cached] private CombatEntity _ownerEntity;
    
    public List<EntityModificatorData> AvailableModificators => new List<EntityModificatorData>(_allAvailableModificators);
    public List<EntityModificatorData> AppliedModificators => _appliedModificators.Keys.ToList();

    public void AddEffect(EntityModificatorData modificatorData)
    {
        EntityModificatior modificatior = _entityModificatorFactory.CreateEntityModificationEffect(modificatorData);
        modificatior.SetEntity(_ownerEntity);
        
        modificatior.Enable();

        if (_appliedModificators.TryGetValue(modificatorData, out var effectList))
        {
            effectList.Add(modificatior);
        }
        else
        {
            _appliedModificators[modificatorData] = new() {modificatior};
        }
    }
    
    public void RemoveEffect(EntityModificatorData modificatorData)
    {
        if (_appliedModificators.TryGetValue(modificatorData, out var effectList))
        {
            EntityModificatior modificatior = effectList[^1];
            modificatior.Disable();
            _appliedModificators[modificatorData].Remove(modificatior);
        }
        else throw new KeyNotFoundException($"Tried to remove an effect {modificatorData.EffectType} but it doesn't exist");
    }
}