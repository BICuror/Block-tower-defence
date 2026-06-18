using UnityEngine;
using Combat;

public sealed class ApplyModificatorInArea : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    private EntityModificatorData _entityModificatorData;
    
    private void Start()
    {
        _areaEntityDetector.AddedItem += ApplyModificator;
        _areaEntityDetector.RemovedItem += RemoveModificator;
        
        _entityModificatorData = Args.GetArgument<EntityModificatorData>("EntityModificatorData");
    }

    private void ApplyModificator(CombatEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_entityModificatorData);
    }
    
    private void RemoveModificator(CombatEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(_entityModificatorData);
    }

    private void OnDestroy()
    {
        _areaEntityDetector.AddedItem -= ApplyModificator;
        _areaEntityDetector.RemovedItem -= RemoveModificator;
    }
}