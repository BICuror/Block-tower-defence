public sealed class InvincibilityEffect : EntityEffect
{
    public override EntityEffectType EffectType => EntityEffectType.Positive;

    
    public override void ApplyToEntity()
    {
        Entity.Health.InvulnerabilityTokenContainer.AddToken();
    }

    public override void RemoveFromEntity()
    {
        Entity.Health.InvulnerabilityTokenContainer.RemoveToken();
    }
}