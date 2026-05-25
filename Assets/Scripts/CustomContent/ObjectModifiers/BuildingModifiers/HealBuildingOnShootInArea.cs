using UnityEngine;
using Cashing;
using Combat;

public sealed class HealBuildingOnShootInArea : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [Cached] private TaskCycle _taskCycle;
    [Cached] private CombatEntity _owner;
    private float _healAmount;
    
    private void Start()
    {
        _healAmount = Args.GetArgument<float>("HealAmount");
        
        _taskCycle.TaskPerformed += TryToActivate;
    }

    private void TryToActivate()
    {
        if (_buildingAreaScaner.IsEmpty) return;

        CombatEntity entity = _buildingAreaScaner.RandomItem;
        
        entity.Health.ReceiveHeal(_healAmount, _owner);
    }

    private void OnDestroy()
    {
        _taskCycle.TaskPerformed -= TryToActivate;
    }
}