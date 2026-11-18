using Cysharp.Threading.Tasks;
using Configs;
using Combat;

namespace ScriptableLogic.Commands.EntityEffects
{
    public sealed class ApplyStatusEffectPermanentCommand : Command, ICommand<CombatEntity>, ICommand
    {
        [Argument("EffectConfig")] private EntityEffectConfig _entityEffectConfig;
        [Argument("Stacks")] private int _stacks;
        
        public UniTask Execute(CombatEntity args)
        {
            args.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_entityEffectConfig.GetEffectType(), _stacks);
            
            return UniTask.CompletedTask;
        }
    
        public UniTask Execute()
        {
            return Execute(Args.Owner);
        }
    }
    
    public sealed class ApplyStatusEffectTemporaryCommand : Command, ICommand<CombatEntity>, ICommand
    {
        [Argument("EffectConfig")] private EntityEffectConfig _entityEffectConfig;
        [Argument("Stacks")] private int _stacks;
        [Argument("Duration")] private float _duration;
        
        public UniTask Execute(CombatEntity args)
        {
            args.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_entityEffectConfig.GetEffectType(), _stacks, _duration);
            
            return UniTask.CompletedTask;
        }
        
        public UniTask Execute()
        {
            return Execute(Args.Owner);
        }
    }
}
