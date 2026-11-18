namespace Combat.ValueModifiers
{
    public sealed class NonLethalDamageValueModifier : ValueModifier
    {
        public override ValueModifierOrder Order => ValueModifierOrder.PostDamageCalculation;

        public override bool AllowValuePass(CombatEntity otherEntity, float value)
        {
            if (otherEntity.Health.GetHp() <= value + CombatUtilities.MINIMAL_NONLETHAL_HEALTH)
            {
                CombatUtilities.TakeMaxNonLethalDamage(otherEntity);

                return false;
            }

            return true;
        }
    }
}