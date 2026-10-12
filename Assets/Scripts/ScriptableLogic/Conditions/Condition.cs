using Configs;

public abstract class Condition : ArgumentsRequester
{
    protected ScriptableLogicFactory.ScriptableLogicConstructorArgs Args;
    
    public void Initialize(ScriptableLogicFactory.ScriptableLogicConstructorArgs args)
    {
        Args = args;

        OnInitialize();
    }
    
    protected virtual void OnInitialize() {}
}

public abstract class SubDomainCondition<T> : Condition where T : SubDomain
{
    protected T SubDomain;
    
    protected override void OnInitialize()
    {
        SubDomain = Args.Domain.GetSubDomain<T>();
    }
}