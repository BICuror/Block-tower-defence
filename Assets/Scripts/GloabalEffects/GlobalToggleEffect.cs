public abstract class GlobalToggleEffect
{
    protected ArgumentsContainer Args;

    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public abstract void Enable();
    public abstract void Disable();
}