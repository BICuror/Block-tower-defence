using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityModificatorsContainer : MonoBehaviour
{
    private readonly Dictionary<EntityModificatorData, List<EntityModificator>> _appliedModificators = new();
    [SerializeField] private List<EntityModificatorData> _allAvailableModificators;
    [Inject] private EntityModificatorFactory _entityModificatorFactory;
    [Cached] private CombatEntity _ownerEntity;
    
    public List<EntityModificatorData> AvailableModificators => new List<EntityModificatorData>(_allAvailableModificators);
    public List<EntityModificatorData> AppliedModificators => _appliedModificators.Keys.ToList();

    public void AddEffect(EntityModificatorData modificatorData)
    {
        EntityModificator modificator = _entityModificatorFactory.CreateEntityModificationEffect(modificatorData);
        modificator.SetEntity(_ownerEntity);
        
        if (!modificator.CanBeApplied()) return;
        
        modificator.Enable();

        if (_appliedModificators.TryGetValue(modificatorData, out var effectList))
        {
            effectList.Add(modificator);
        }
        else
        {
            _appliedModificators[modificatorData] = new() {modificator};
        }
    }
    
    public void RemoveEffect(EntityModificatorData modificatorData)
    {
        if (_appliedModificators.TryGetValue(modificatorData, out var effectList))
        {
            EntityModificator modificator = effectList[^1];
            modificator.Disable();
            _appliedModificators[modificatorData].Remove(modificator);
        }
    }
}