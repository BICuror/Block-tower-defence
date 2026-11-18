using Cysharp.Threading.Tasks;
using Configs;
using Combat;

namespace ScriptableLogic.Commands.EntityEffects
{
    public sealed class RemoveStatusEffectStacksCommand : Command, ICommand<CombatEntity>
    {
        [Argument("EffectConfig")] private EntityEffectConfig _entityEffectConfig;
        [Argument("Stacks")] private int _stacks;

        public UniTask Execute(CombatEntity args)
        {
            args.ComponentsContainer.Get<EntityEffectManager>()
                .RemoveEffect(_entityEffectConfig.GetEffectType(), _stacks);

            return UniTask.CompletedTask;
        }
    }
}