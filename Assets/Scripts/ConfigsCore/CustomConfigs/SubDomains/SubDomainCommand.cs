public abstract class SubDomainCommand<T> : Command where T : SubDomain
{
    protected T SubDomain;
    
    protected override void OnInitialize()
    {
        SubDomain = Args.Domain.GetSubDomain<T>();
    }
}