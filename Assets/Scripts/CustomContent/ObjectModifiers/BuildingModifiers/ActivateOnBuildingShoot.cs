using UnityEngine;
using Cashing;
using Combat;

using Random = UnityEngine.Random;

public sealed class ActivateOnBuildingShoot : MonoBehaviour
{
    [Range(0, 100)] [SerializeField] private float _chance;
    [SerializeField] private AreaScanerController _areaScanerController;
    [SerializeField] private BuildingAreaScaner _buildingAreaScaner;
    [SerializeField] private ActivationType _activationType;
    [Cached] private CombatEntity _combatEntity;
    [Cached] private TaskCycle _taskCycle;
    
    private void Start()
    {
        _combatEntity.ComponentsContainer.Get<AreaManager>().AddAreaScanerController(_areaScanerController);
        _taskCycle.TaskPerformed += TryToActivate;
    }

    private void TryToActivate()
    {
        if (_buildingAreaScaner.IsEmpty) return;

        if (Random.Range(0, 100) > _chance) return;

        CombatEntity entity = _buildingAreaScaner.RandomItem;
        
        switch (_activationType)
        {
            case ActivationType.Activation: entity.Activate(); break;
            case ActivationType.Cycle:
            {
                if (entity.ComponentsContainer.Has<TaskCycle>())
                {
                    entity.ComponentsContainer.Get<TaskCycle>().PerformTask();
                }
            } break;
        }
    }

    private void OnDestroy()
    {
        _combatEntity.ComponentsContainer.Get<AreaManager>().RemoveAreaScanerController(_areaScanerController);
        _taskCycle.TaskPerformed -= TryToActivate;
    }

    private enum ActivationType
    {
        Activation,
        Cycle
    }
}