using UnityEngine;
using Cashing;
using Combat;

public sealed class HealBuildingOnShootInArea : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] private float _healPercent;
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [Cached] private TaskCycle _taskCycle;
    
    private void Start()
    {
        _taskCycle.TaskPerformed += TryToActivate;
    }

    private void TryToActivate()
    {
        if (_buildingAreaScaner.IsEmpty) return;

        CombatEntity entity = _buildingAreaScaner.RandomItem;
        
        entity.Health.ReceivePercentHeal(_healPercent);
    }

    private void OnDestroy()
    {
        _taskCycle.TaskPerformed -= TryToActivate;
    }
}