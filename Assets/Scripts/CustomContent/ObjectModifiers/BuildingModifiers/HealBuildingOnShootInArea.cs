using ScriptableLogic.ArgumentItem;
using Cysharp.Threading.Tasks;
using Configs;
using Combat;

namespace ScriptableLogic.Commands
{
    public sealed class HealCommand : Command, ICommand, ICommand<CombatEntity>
    {
        [ArgumentItem("HealAmount")] private ValueArgumentItem _healAmount;
        
        public UniTask Execute()
        {
            return Execute(Args.Owner);
        }

        public UniTask Execute(CombatEntity args)
        {
            args.Health.ReceiveHeal(_healAmount.GetValueModificationContainer(), Args.Owner);
            
            return UniTask.CompletedTask;
        }
    }
}