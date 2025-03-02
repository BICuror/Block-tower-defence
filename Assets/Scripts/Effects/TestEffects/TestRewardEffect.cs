using Zenject;

public sealed class TestRewardEffect : RewardEffect
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void GrantReward()
    {
        _waveStateMachine.TransitionIntoAttack();
    }
}