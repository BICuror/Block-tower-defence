using ScriptableLogic.ArgumentItem;
using Cysharp.Threading.Tasks;
using Configs;
using Combat;

namespace ScriptableLogic.Commands.Combat
{
    public sealed class DealNonLethalDamageCommand : Command, ICommand<CombatEntity>, ICommand
    {
        [ArgumentItem("DamageAmount")] private ValueArgumentItem _damageAmount;
    
        public UniTask Execute()
        {
            return Execute(Args.Owner);
        }
        
        public UniTask Execute(CombatEntity args)
        {        
            CombatUtilities.TryTakeNonLethalDamage(args, _damageAmount.GetValue());
            
            return UniTask.CompletedTask;
        }
    }
}