using UnityEngine;
using Combat;

public sealed class ApplyModificatorInArea : MonoBehaviour
{
    [SerializeField] private EntityModificatorData _entityModificatorData;
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    
    private void Awake()
    {
        _areaEntityDetector.AddedItem += ApplyModificator;
        _areaEntityDetector.RemovedItem += RemoveModificator;
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