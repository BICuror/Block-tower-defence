using Configs;

namespace Combat.ValueModifiers
{
    public sealed class ModifyIfHasEntityEffectValueModifier : DefaultConditionalValueModifier
    {
        [Argument("EvaluateOtherEntityHealth")] private bool _evaluateOtherEntityHealth;
        [Argument("EntityEffectConfig")] private EntityEffectConfig _entityEffectConfig;
        
        protected override bool ConditionIsMet(CombatEntity otherEntity, float value)
        {
            if (_evaluateOtherEntityHealth)
            {
                return otherEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(_entityEffectConfig.GetEffectType());
            }

            return OwnerEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(_entityEffectConfig.GetEffectType());
        }
    }
}