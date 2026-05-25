using Combat;

public sealed class MarkEffect : EntityEffect
{
    private MarkDamageModifier _markDamageModifier;
    
    public override EntityEffectType EffectType => EntityEffectType.Positive;

    protected override void OnInitialized()
    {
        _markDamageModifier = new MarkDamageModifier();
        _markDamageModifier.Initialize(ArgumentsContainer.GetArgument<float>("IncomingDamageModifier"));
    }

    public override void ApplyToEntity()
    {
        Entity.ValueModifierContainer.DamageReceiverContainer.Add(_markDamageModifier);
    }

    public override void RemoveFromEntity()
    {
        Entity.ValueModifierContainer.DamageReceiverContainer.Remove(_markDamageModifier);
    }

    private sealed class MarkDamageModifier : DamageModifier
    {
        private float _damageModifier;
        
        public void Initialize(float damageModifier)
        {
            _damageModifier = damageModifier;
        }
        
        public override float Modify(CombatEntity otherEntity, float value)
        {
            return _damageModifier * value;
        }
    }
}