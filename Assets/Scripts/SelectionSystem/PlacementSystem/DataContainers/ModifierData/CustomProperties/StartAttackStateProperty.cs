using Zenject;

public sealed class StartAttackStateProperty : ItemProperty
{
    [Inject] private WaveStateMachine _waveStateMachine;

    public override void Enable()
    {
        _waveStateMachine.TransitionIntoAttack();
    }

    public override void Disable() { }
}