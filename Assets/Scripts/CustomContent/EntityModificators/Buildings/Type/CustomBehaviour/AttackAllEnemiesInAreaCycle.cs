using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Combat;

public sealed class AttackAllEnemiesInAreaCycle : EntityModificator
{
    private float _timePeriod;
    private float _damage;
    
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private AreaEntityDetector _areaEntityDetector;
    private bool _attackCycleIsActive;
    
    public override void Enable()
    {
        _timePeriod = Args.GetArgument<float>("TimePeriod");
        _damage = Args.GetArgument<float>("Damage");

        _areaEntityDetector = Entity.ComponentsContainer.Get<AreaEntityDetector>();
        
        _areaEntityDetector.AddedItem += TryActivateAttack;
        _areaEntityDetector.RemovedItem += TryDeactivateAttack;
    }

    private void TryActivateAttack(CombatEntity _)
    {
        if (_attackCycleIsActive || _areaEntityDetector.IsEmpty) return;
        
        StartAttackCycle().Forget();
    }

    private void TryDeactivateAttack(CombatEntity _)
    {
        if (!_attackCycleIsActive || !_areaEntityDetector.IsEmpty) return;
        
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    private async UniTask StartAttackCycle()
    {
        if (_attackCycleIsActive) return;

        _attackCycleIsActive = true;

        while (true)
        {
            try
            {
                await UniTask.WaitForSeconds(_timePeriod, cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                break;
            }

            AttackAllEnemiesInArea();
        }

        _attackCycleIsActive = false;
    }

    private void AttackAllEnemiesInArea()
    {
        List<CombatEntity> capturedEntities = new(_areaEntityDetector.GetList());
        
        foreach (CombatEntity combatEntity in capturedEntities)
        {
            combatEntity.Health.ReceiveEnemyDamage(_damage, Entity);
        }
    }
    
    public override void Disable()
    {
        _areaEntityDetector.AddedItem -= TryActivateAttack;
        _areaEntityDetector.RemovedItem -= TryDeactivateAttack;
    }
}