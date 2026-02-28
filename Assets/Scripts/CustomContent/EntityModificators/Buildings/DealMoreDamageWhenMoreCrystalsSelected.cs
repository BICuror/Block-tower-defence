using UnityEngine;
using Zenject;

public sealed class DealMoreDamageWhenMoreCrystalsSelected : EntityModificator
{
    [Inject] private ItemsContainer _itemsContainer;
    private EntityCanvasIcon _entityCanvasIcon;
    private StatModifier _statModifier = new();
    
    public override void Enable()
    {
        _itemsContainer.ItemAdded += UpdateStatModifier;
        _itemsContainer.ItemRemoved += UpdateStatModifier;
        
        Entity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);

        UpdateStatModifier(null);
    }

    public override void Disable()
    {
        _itemsContainer.ItemAdded -= UpdateStatModifier;
        _itemsContainer.ItemRemoved -= UpdateStatModifier;
        
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
        
        if (_entityCanvasIcon) RemoveIcon(_entityCanvasIcon);
    }
    
    private void UpdateStatModifier(Item _)
    {
        bool shouldBeEnabled = _itemsContainer.ContainedItems.Count >= Args.GetArgument<int>("Threshold");
        
        if (shouldBeEnabled)
        {
            _statModifier.SetMultiplier(Args.GetArgument<float>("Multiplier"));
        }
        else
        {
            _statModifier.SetMultiplier(0f);
        }
        
        UpdateUI(shouldBeEnabled);
    }
    
    private void UpdateUI(bool shouldBeEnabled)
    {
        if (shouldBeEnabled && !_entityCanvasIcon) _entityCanvasIcon = AddIcon(false);
        else if (!shouldBeEnabled && _entityCanvasIcon) RemoveIcon(_entityCanvasIcon);
    }
}