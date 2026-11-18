using Cysharp.Threading.Tasks;
using Combat.Stats;
using Configs;

namespace ScriptableLogic.Commands
{
    public sealed class ChangeStatCommand : StatCommand
    {
        [Argument("FlatChange")] private float _flatChange;
        [Argument("MultiplierChange")] private float _multiplierChange;
        
        private StatModifier _statModifier;
        
        public override UniTask Enable()
        {
            _statModifier = new StatModifier(_flatChange, _multiplierChange);
            
            GetOwnerStat().AddStatModifier(_statModifier);
            
            return UniTask.CompletedTask;
        }
    
        public override UniTask Disable()
        {
            GetOwnerStat().RemoveStatModifier(_statModifier);
            
            return UniTask.CompletedTask;
        }
    }

    public abstract class StatCommand : Command, INonExecutableCommand
    {
        [Argument("StatConfig")] protected StatConfig StatConfig;

        protected Stat GetOwnerStat() => Args.Owner.StatContainer.Get(StatConfig.GetStatType());
    }
}