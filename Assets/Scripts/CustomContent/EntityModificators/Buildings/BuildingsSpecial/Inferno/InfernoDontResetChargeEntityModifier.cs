using Zenject;

public sealed class InfernoDontResetChargeEntityModifier : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateStarted += Entity.ComponentsContainer.Get<InfernoTower>().ResetCharge;
        
        Entity.ComponentsContainer.Get<InfernoTower>().OnEnemyChanged.AddBehaviour(new DoNothing(), BehaviourType.Override);
    }

    public override void Disable()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateStarted -= Entity.ComponentsContainer.Get<InfernoTower>().ResetCharge;
        
        Entity.ComponentsContainer.Get<InfernoTower>().OnEnemyChanged.RemoveOverrideBehaviour();
    }
    
    private sealed class DoNothing : CombatBehaviour
    {
        public override void Execute() {}
    }
}