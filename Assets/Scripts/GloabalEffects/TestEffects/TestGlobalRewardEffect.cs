using Zenject;

public sealed class TestGlobalRewardEffect : GlobalRewardEffect
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void GrantReward()
    {
        _waveStateMachine.TransitionIntoAttack();
    }
}