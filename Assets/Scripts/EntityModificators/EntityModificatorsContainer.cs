using System;
using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityModificatorsContainer : MonoBehaviour
{
    private readonly ListDictionary<EntityModificatorData, EntityModificator> _appliedModificators = new();
    [SerializeField] private List<EntityModificatorData> _initialModificatorDatas;
    [SerializeField] private List<EntityModificatorData> _allAvailableModificators;
    [Inject] private EntityModificatorFactory _entityModificatorFactory;
    [Cached] private CombatEntity _ownerEntity;

    public List<EntityModificatorData> AvailableModificators => new List<EntityModificatorData>(_allAvailableModificators);
    public List<EntityModificatorData> AppliedModificators => _appliedModificators.GetAllKeys();

    private void Start()
    {
        _initialModificatorDatas.ForEach(modificatorData =>
        {
            AddEffect(modificatorData);
        });   
    }

    public int GetModificatorsAmount(EntityModificatorData modificatorData) => _appliedModificators.Get(modificatorData).Count;
    
    public void AddEffect(EntityModificatorData modificatorData)
    {
        EntityModificator modificator = _entityModificatorFactory.CreateEntityModificationEffect(modificatorData);
        modificator.SetEntity(_ownerEntity);
        
        if (!modificator.CanBeApplied()) return;
        
        modificator.Enable();

        _appliedModificators.Add(modificatorData, modificator);
    }

    public bool Has(Type modificatorType)
    {
        return _appliedModificators.GetAllKeys().Exists(data => data.EffectType == modificatorType);
    }

    public void RemoveEffect(Type modificatorType)
    {
        EntityModificatorData modificatorData = _appliedModificators.GetAllKeys().Find(data => data.EffectType == modificatorType);
        
        if (modificatorData == null) return;
        
        RemoveEffect(modificatorData);
    }
    
    public void RemoveEffect(EntityModificatorData modificatorData)
    {
        if (_appliedModificators.Contains(modificatorData))
        {
            EntityModificator modificator = _appliedModificators.Remove(modificatorData);
            
            modificator.Disable();
        }
    }
}