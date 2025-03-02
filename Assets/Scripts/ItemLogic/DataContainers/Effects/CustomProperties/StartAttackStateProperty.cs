using Zenject;

public sealed class StartAttackStateProperty : RewardEffect
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void GrantReward()
    {
        _waveStateMachine.TransitionIntoAttack();
    }
}