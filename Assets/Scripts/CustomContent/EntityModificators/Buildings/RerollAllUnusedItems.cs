using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using Zenject;

public sealed class RerollAllUnusedItems : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private ItemsContainer _itemsContainer;
    [Inject] private ItemFactory _itemFactory;
    private EntityCanvasIcon _entityCanvasIcon;
    
    public override void Enable()
    {
        _waveStateMachine.StateStarted += UpdateAbilityState;
        EnableRerollAbility();
    }

    public override void Disable()
    {
        _waveStateMachine.StateStarted -= UpdateAbilityState;
        DisableRerollAbility();
    }

    private void UpdateAbilityState(WaveState waveState)
    {
        if (waveState == WaveState.Attack)
        {
            DisableRerollAbility();
        }
        else if (waveState == WaveState.Idle)
        {
            EnableRerollAbility();
        }
    }
    
    private void EnableRerollAbility()
    {
        _entityCanvasIcon = AddIcon(false);
        
        Entity.Activated += TryActivateReroll;
    }
    
    private void DisableRerollAbility()
    {
        if (_entityCanvasIcon) RemoveIcon(_entityCanvasIcon);
        Entity.Activated -= TryActivateReroll;
    }

    private void TryActivateReroll()
    {
        if (_itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList().Count > 0)
        {
            RerollUnusedItems();
            RemoveIcon(_entityCanvasIcon);
            
            Entity.Activated -= TryActivateReroll;
        }
    }

    private void RerollUnusedItems()
    {
        int destroyedItems = 0;
        List<int> itemStrengths = new();

        List<Item> unusedItems = _itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList();
        
        unusedItems.ForEach(unusedItem =>
        {
            if (!unusedItem.ToggleEffectDatas.Exists(data => data.InstanceItemTypeContainers.Exists(typeContainer => typeContainer.InstanceType == typeof(StartWaveGlobalToggleEffect))))
            {
                destroyedItems++;
                itemStrengths.Add(unusedItem.Charges);
                _itemFactory.RemoveAndDestroyItem(unusedItem);
            }
        });

        for (int i = 0; i < destroyedItems; i++)
        {
            _itemFactory.CreateItem(itemStrengths[i], Entity.transform.position).Forget();
        }
    }
}