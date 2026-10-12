using Cysharp.Threading.Tasks;
using Configs;

public abstract class Command : ArgumentsRequester
{
    protected ScriptableLogicFactory.ScriptableLogicConstructorArgs Args;
    
    private ConditionalContainer _conditionalContainer;

    public ConditionalContainer ConditionalContainer => _conditionalContainer;
    
    public void Initialize(ScriptableLogicFactory.ScriptableLogicConstructorArgs args, ConditionalContainer conditionalContainer)
    {
        _conditionalContainer = conditionalContainer;
        Args = args;
        
        OnInitialize();
    }
    
    protected virtual void OnInitialize() {}
    
    public virtual UniTask Enable() => UniTask.CompletedTask;

    public virtual UniTask Disable() => UniTask.CompletedTask;
}