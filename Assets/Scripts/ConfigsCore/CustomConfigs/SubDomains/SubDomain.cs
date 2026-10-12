using Cysharp.Threading.Tasks;
using Configs;

public abstract class SubDomain : ArgumentsRequester
{
    protected ScriptableLogicFactory.ScriptableLogicConstructorArgs Args;
    
    public void Initialize(ScriptableLogicFactory.ScriptableLogicConstructorArgs args)
    {
        Args = args;
    }
    
    protected virtual void OnInitialize() {}
    
    public virtual UniTask Enable() => UniTask.CompletedTask;
    public virtual UniTask Disable() => UniTask.CompletedTask;
}