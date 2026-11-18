using Combat.ValueModifiers;
using Combat;

public sealed class MarkEffect : EntityEffect
{
    public override EntityEffectType EffectType => EntityEffectType.Negative;

    protected override void OnInitialized()
    {
        _markValueModifier.Initialize(ArgumentsContainer.GetArgument<float>("IncomingDamageModifier"));
    }

    public override void ApplyToEntity()
    {
        Entity.ValueModifierContainer.DamageReceiverContainer.Add(_markValueModifier);
    }

    public override void RemoveFromEntity()
    {
        Entity.ValueModifierContainer.DamageReceiverContainer.Remove(_markValueModifier);
    }
}