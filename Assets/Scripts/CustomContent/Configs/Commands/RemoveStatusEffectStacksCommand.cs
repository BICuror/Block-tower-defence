using Cysharp.Threading.Tasks;
using Configs;
using Combat;

public sealed class RemoveStatusEffectStacksCommand : Command, ICommand<CombatEntity>
{
    [Argument("EffectData")] private EntityEffectData _entityEffectData;
    [Argument("Stacks")] private int _stacks;
    
    public UniTask Execute(CombatEntity args)
    {
        args.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(_entityEffectData.GetEffectType(), _stacks);
        
        return UniTask.CompletedTask;
    }
}