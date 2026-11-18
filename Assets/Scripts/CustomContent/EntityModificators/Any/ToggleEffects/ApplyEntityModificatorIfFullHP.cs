using Combat.Entity.Modificators;

public sealed class ApplyEntityModificatorIfFullHP : EntityModificator
{
    private EntityModificatorConfig _entityModificatorConfig;
    private bool _applied;
    
    public override void Enable()
    {
        _entityModificatorConfig = Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig");

        UpdateModificatorState();

        Entity.Health.Damaged += UpdateModificatorState;
        Entity.Health.Healed += UpdateModificatorState;
    }

    public override void Disable()
    {
        Entity.Health.Damaged -= UpdateModificatorState;
        Entity.Health.Healed -= UpdateModificatorState;

        if (_applied) RemoveModificator();
    }
    
    private void UpdateModificatorState()
    {
        if (Entity.Health.IsFullHp() && !_applied) ApplyModificator();
        else if (!Entity.Health.IsFullHp() && _applied) RemoveModificator();
    }

    private void ApplyModificator()
    {
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_entityModificatorConfig);
        _applied = true;
    }

    private void RemoveModificator()
    {
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(_entityModificatorConfig);
        _applied = false;
    }
}