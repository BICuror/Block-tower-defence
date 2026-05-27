using System.Collections.Generic;
using UnityEngine;
using Combat;

public sealed class AttackAllEnemiesInAreaCycle : EntityModificator
{
    private TaskRechargeDuration _taskRechargeDuration;
    private float _damage;
    
    private AreaEntityDetector _areaEntityDetector;
    private BuildingCycleCore _taskCycle;
    
    public override void Enable()
    {
        _damage = Args.GetArgument<float>("Damage");
        
        _taskRechargeDuration = new TaskRechargeDuration();
        _taskRechargeDuration.SetDefault(Args.GetArgument<float>("TimePeriod"));

        _taskCycle = new BuildingCycleCore((BuildingEntity)Entity, _taskRechargeDuration);
        _taskCycle.TaskPerformed = AttackAllEnemiesInArea;
        _taskCycle.Enable();
        _taskCycle.TryCycle();

        _areaEntityDetector = Entity.ComponentsContainer.Get<AreaEntityDetector>();
        
        _areaEntityDetector.AddedItem += TryActivateAttack;
        _areaEntityDetector.RemovedItem += TryDeactivateAttack;
    }

    private void TryActivateAttack(CombatEntity _)
    {
        if (_areaEntityDetector.IsEmpty) return;

        Debug.Log("TRIED INVOKEING");
        
        _taskCycle.TryCycle();
    }

    private void TryDeactivateAttack(CombatEntity _)
    {
        if (!_areaEntityDetector.IsEmpty) return;
        
        _taskCycle.StopRechargeProcess();
    }
    
    private void AttackAllEnemiesInArea()
    {
        List<CombatEntity> capturedEntities = new(_areaEntityDetector.GetList());
        
        capturedEntities.ForEach(entity => entity.Health.ReceiveEnemyDamage(_damage, Entity));
    }
    
    public override void Disable()
    {
        _areaEntityDetector.AddedItem -= TryActivateAttack;
        _areaEntityDetector.RemovedItem -= TryDeactivateAttack;
        _taskCycle.StopRechargeProcess();
        _taskCycle.Disable();
    }
}