public sealed class ApplyEntityModificatorIfFullHP : EntityModificator
{
    private EntityModificatorData _entityModificatorData;
    private bool _applied;
    
    public override void Enable()
    {
        _entityModificatorData = Args.GetArgument<EntityModificatorData>("EntityModificatorData");

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
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_entityModificatorData);
        _applied = true;
    }

    private void RemoveModificator()
    {
        Entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(_entityModificatorData);
        _applied = false;
    }
}