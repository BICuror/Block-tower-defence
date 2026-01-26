using Zenject;

public sealed class TakeDamageUponPlacement : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    private float _damagePercent;
    
    public override void Enable()
    {
        _damagePercent = Args.GetArgument<float>("DamagePercent");
        
        Entity.Draggable.Placed += DamageEntity;
    }

    public override void Disable()
    {
        Entity.Draggable.Placed -= DamageEntity;
    }

    private void DamageEntity()
    {
        if (_waveStateMachine.CurrentState != WaveState.Attack) return;

        CombatUtilities.TryTakeNonLethalDamage(Entity, _damagePercent * Entity.Health.GetMaxHp());
    }
}