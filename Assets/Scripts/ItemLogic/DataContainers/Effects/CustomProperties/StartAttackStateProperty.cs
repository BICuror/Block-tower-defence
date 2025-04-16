using Zenject;

public sealed class StartAttackStateProperty : GlobalRewardEffect
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void GrantReward()
    {
        _waveStateMachine.TransitionIntoAttack();
    }
}