public abstract class EffectApperanceCondition
{
    protected ArgumentsContainer Args;

    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public abstract bool CanAppear();
}