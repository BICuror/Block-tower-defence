using Combat;

public sealed class InvincibilityEffect : EntityEffect
{
    public override void ApplyToEntity()
    {
        Entity.DamageModifierContainer.ReciverContainer.Add<InvincibilityDamageModifier>();
    }

    public override void RemoveFromEntity()
    {
        Entity.DamageModifierContainer.ReciverContainer.Remove<InvincibilityDamageModifier>();
    }
}