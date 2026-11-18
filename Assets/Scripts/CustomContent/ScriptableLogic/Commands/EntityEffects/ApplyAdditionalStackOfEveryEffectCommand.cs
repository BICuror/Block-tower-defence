using Cysharp.Threading.Tasks;
using Configs;
using System;
using Combat;

namespace ScriptableLogic.Commands.EntityEffects
{
    public sealed class ApplyAdditionalStackOfEveryEffectCommand : Command, ICommand, ICommand<CombatEntity>
    {
        [Argument("AdditionalStackAmount")] private int _additionalStackAmount;
        [Argument("AdditionalStackDuration")] private float _additionalStackDuration;
        
        [Argument("ExcludePositiveEffects")] private bool _excludePositiveEffects;
        [Argument("ExcludeNegativeEffects")] private bool _excludeNegativeEffects;

        public UniTask Execute() => Execute(Args.Owner);
        
        public UniTask Execute(CombatEntity args)
        {
            EntityEffectManager effectManager = args.ComponentsContainer.Get<EntityEffectManager>();

            foreach (Type appliedEffectType in effectManager.AppliedEffects.Keys)
            {
                if (effectManager.AppliedEffects[appliedEffectType].EffectType == EntityEffectType.Negative && _excludeNegativeEffects) continue;

                if (effectManager.AppliedEffects[appliedEffectType].EffectType == EntityEffectType.Positive && _excludePositiveEffects) continue;


                effectManager.TryApplyTemporaryEffect(appliedEffectType, _additionalStackAmount, _additionalStackDuration);
            }

            return UniTask.CompletedTask;
        }
    }
}
