using UnityEngine;

namespace ScriptableLogic.ArgumentItem
{
    public abstract class ArgumentItem : MonoBehaviour
    {
        protected ScriptableLogicFactory.ScriptableLogicConstructorArgs Args;
        
        public void Initialize(ScriptableLogicFactory.ScriptableLogicConstructorArgs args)
        {
            Args = args;
        }
    }
}