using ScriptableLogic.SubDomains;

namespace ScriptableLogic.Conditions
{
    public abstract class SubDomainCondition<T> : Condition where T : SubDomain
    {
        protected T SubDomain;
        
        protected override void OnInitialize()
        {
            SubDomain = Args.Domain.GetSubDomain<T>();
        }
    }
}