public abstract class RewardEffect
{
    protected ArgumentsContainer Args;

    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public abstract void GrantReward();
}