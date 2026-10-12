using Cysharp.Threading.Tasks;
using Configs;
using Combat;

public sealed class ApplyStatusEffectPermamentCommand : Command, ICommand<CombatEntity>, ICommand
{
    [Argument("EffectData")] private EntityEffectData _entityEffectData;
    [Argument("Stacks")] private int _stacks;
    
    public UniTask Execute(CombatEntity args)
    {
        args.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_entityEffectData.GetEffectType(), _stacks);
        
        return UniTask.CompletedTask;
    }

    public UniTask Execute()
    {
        return Execute(Args.Owner);
    }
}

public sealed class ApplyStatusEffectTemporaryCommand : Command, ICommand<CombatEntity>, ICommand
{
    [Argument("EffectData")] private EntityEffectData _entityEffectData;
    [Argument("Stacks")] private int _stacks;
    [Argument("Duration")] private float _duration;
    
    public UniTask Execute(CombatEntity args)
    {
        args.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_entityEffectData.GetEffectType(), _stacks, _duration);
        
        return UniTask.CompletedTask;
    }
    
    public UniTask Execute()
    {
        return Execute(Args.Owner);
    }
}