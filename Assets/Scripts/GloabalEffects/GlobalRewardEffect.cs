public abstract class GlobalRewardEffect
{
    protected ArgumentsContainer Args;

    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public abstract void GrantReward();
}