using Configs;

namespace ScriptableLogic.Conditions
{
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
}