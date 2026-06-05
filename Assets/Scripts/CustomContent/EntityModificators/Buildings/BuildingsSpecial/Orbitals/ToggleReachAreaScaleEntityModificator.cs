public sealed class ToggleReachAreaScaleEntityModificator : EntityModificator
{
    private EntityCanvasAbilityIcon _abilityIcon;
    private StatModifier _statModifier = new();
    
    public override void Enable()
    {
        Entity.StatContainer.Get<ReachAreaScale>().AddStatModifier(_statModifier);
        Entity.Activated += ChangeReachAreaScale;

        _abilityIcon = AddAbilityIcon(1);
    }

    public override void Disable()
    {
        Entity.StatContainer.Get<ReachAreaScale>().RemoveStatModifier(_statModifier);
        Entity.Activated -= ChangeReachAreaScale;
        
        RemoveAbilityIcon(_abilityIcon);
    }
    
    private void ChangeReachAreaScale()
    {
        ReachAreaScale areaScale = Entity.StatContainer.Get<ReachAreaScale>();

        if (areaScale.RoundedValue > 1)
        {
            _statModifier.SetFlat(_statModifier.Flat - 1);
        }
        else
        {
            _statModifier.SetFlat(0);
        }
    } 
}