using UnityEngine;
using Cashing;
using Combat;

using Random = UnityEngine.Random;

public sealed class ActivateOnBuildingShoot : MonoBehaviour
{
    [Range(0, 100)] [SerializeField] private float _chance;
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [SerializeField] private ActivationType _activationType;
    [Cached] private TaskCycle _taskCycle;
    
    private void Start()
    {
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
        _taskCycle.TaskPerformed -= TryToActivate;
    }

    private enum ActivationType
    {
        Activation,
        Cycle
    }
}