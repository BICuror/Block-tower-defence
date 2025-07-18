using Zenject;

public sealed class DealMoreDamageWhenMoreCrystalsSelected : EntityModificator
{
    [Inject] private ItemsContainer _itemsContainer;
    private StatModifier _statModifier = new();
    
    private void UpdateStatModifier(Item _)
    {
        if (_itemsContainer.ContainedItems.Count >= Args.GetArgument<int>("Threshold"))
        {
            _statModifier.SetMultiplier(Args.GetArgument<float>("Multiplier"));
        }
        else
        {
            _statModifier.SetMultiplier(0f);
        }
    }

    public override void Enable()
    {
        _itemsContainer.ItemAdded += UpdateStatModifier;
        _itemsContainer.ItemRemoved += UpdateStatModifier;
        
        Entity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);
    }

    public override void Disable()
    {
        _itemsContainer.ItemAdded += UpdateStatModifier;
        _itemsContainer.ItemRemoved += UpdateStatModifier;
        
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
    }
}