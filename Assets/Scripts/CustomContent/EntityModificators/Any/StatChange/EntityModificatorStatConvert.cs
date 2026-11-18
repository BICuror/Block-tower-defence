using ScriptableLogic.Conditions;
using Cysharp.Threading.Tasks;
using Combat.Stats;
using Configs;

namespace ScriptableLogic.Commands
{
    public sealed class ConvertStatCommand : StatCommand, INonExecutableCommand
    {
        [Argument("ConvertedFromStatConfig")] private StatConfig _convertedFromStatConfig;
        private Stat _convertedStat;
        
        public override UniTask Enable()
        {
            Stat convertedFromStat = Args.Owner.StatContainer.Get(_convertedFromStatConfig.GetStatType());
            
            Args.Owner.StatContainer.AddStat(StatConfig.GetStatType(), _convertedStat.Value);
            convertedFromStat.ValueChanged += SetDefaultValue;
            
            return UniTask.CompletedTask;
        }

        private void SetDefaultValue(float value)
        {
            _convertedStat.SetDefault(value);    
        }
        
        public override UniTask Disable()
        {            
            Stat convertedFromStat = Args.Owner.StatContainer.Get(_convertedFromStatConfig.GetStatType());
            
            convertedFromStat.ValueChanged -= SetDefaultValue;
            
            return UniTask.CompletedTask;
        }
    }

    public sealed class HasStatCondition : Condition, ICondition
    {
        [Argument("RequiredStatConfig")] private StatConfig _requiredStatConfig;
        
        public bool GetValue()
        {
            return Args.Owner.StatContainer.Has(_requiredStatConfig.GetStatType());
        }
    }
}