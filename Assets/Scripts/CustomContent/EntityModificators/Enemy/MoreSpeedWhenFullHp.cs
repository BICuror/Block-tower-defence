public sealed class MoreSpeedWhenFullHp : EntityModificator
{
    private StatModifier _statModifier;
        
    public override void Enable()
    {
        _statModifier = new StatModifier(multiplier: -Args.GetArgument<float>("SpeedMultiplier"));

        Entity.Health.Damaged += UpdateModifier;
        UpdateModifier();
    }

    private void UpdateModifier()
    {
        if (Entity.StatContainer.Get<Speed>().IsApplied(_statModifier))
        {
            if (!Entity.Health.IsFullHp()) Entity.StatContainer.Get<Speed>().RemoveStatModifier(_statModifier);           
        }
        else
        {
            if (Entity.Health.IsFullHp()) Entity.StatContainer.Get<Speed>().AddStatModifier(_statModifier);
        }
    }

    public override void Disable()
    {
        if (Entity.StatContainer.Get<Speed>().IsApplied(_statModifier))
        {
            if (Entity.Health.IsFullHp()) Entity.StatContainer.Get<Speed>().RemoveStatModifier(_statModifier);
        }
        
        Entity.Health.Damaged -= UpdateModifier;
    }
}