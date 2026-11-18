using Cysharp.Threading.Tasks;

namespace ScriptableLogic.Commands
{
    public sealed class InvokeActionCommand : Command, ICommand
    {
        public UniTask Execute()
        {
            Args.Owner.ComponentsContainer.Get<TaskCycle>().PerformTask();
            
            return UniTask.CompletedTask;
        }
    }
}