using Cysharp.Threading.Tasks;
using Combat.Stats;
using Configs;

namespace ScriptableLogic.Commands.Combat.Inferno
{
    public sealed class ChangeInfernoChargeCommand : Command, ICommand
    {
        [Argument("ChargeChangePercent")] private float _chargeChangePercent;
        
        public UniTask Execute()
        {
            Args.Owner.ComponentsContainer.Get<InfernoTower>().ChangeChargePercent(Args.Owner.StatContainer.Get<ChargeDuration>().Value * _chargeChangePercent);
            
            return UniTask.CompletedTask;
        }
    }
}