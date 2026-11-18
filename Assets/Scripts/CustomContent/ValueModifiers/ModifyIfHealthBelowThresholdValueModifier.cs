using Configs;

namespace Combat.ValueModifiers
{
    public sealed class ModifyIfHealthBelowThresholdValueModifier : DefaultConditionalValueModifier
    {
        [Argument("EvaluateOtherEntityHealth")] private bool _evaluateOtherEntityHealth;
        [Argument("PercentThreshold")] private float _percentThreshold;

        protected override bool ConditionIsMet(CombatEntity otherEntity, float value)
        {
            if (_evaluateOtherEntityHealth)
            {
                return otherEntity.Health.GetHpPercent() <= _percentThreshold;
            }

            return OwnerEntity.Health.GetHpPercent() <= _percentThreshold;
        }
    }
}