using Combat.Entity.Modificators;
using UnityEngine;
using Combat;

public sealed class ApplyModificatorInArea : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    private EntityModificatorConfig _entityModificatorConfig;
    
    private void Start()
    {
        _areaEntityDetector.AddedItem += ApplyModificator;
        _areaEntityDetector.RemovedItem += RemoveModificator;
        
        _entityModificatorConfig = Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig");
    }

    private void ApplyModificator(CombatEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_entityModificatorConfig);
    }
    
    private void RemoveModificator(CombatEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(_entityModificatorConfig);
    }

    private void OnDestroy()
    {
        _areaEntityDetector.AddedItem -= ApplyModificator;
        _areaEntityDetector.RemovedItem -= RemoveModificator;
    }
}