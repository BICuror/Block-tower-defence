using Cysharp.Threading.Tasks;
using Combat.Stats;

namespace ScriptableLogic.Commands.Combat
{
    public sealed class CreateStatCommand : Command, INonExecutableCommand
    {
        private Stat _createdStat;
        
        public override UniTask Enable()
        {
            
            return UniTask.CompletedTask;
        }
    
        public override UniTask Disable()
        {
            
            return UniTask.CompletedTask;
        }
    }
}
