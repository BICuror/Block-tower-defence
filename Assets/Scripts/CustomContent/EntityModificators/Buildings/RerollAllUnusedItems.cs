using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.IngameUI;
using Combat.Items;
using System.Linq;
using Zenject;

public sealed class RerollAllUnusedItems : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private ItemsContainer _itemsContainer;
    [Inject] private ItemFactory _itemFactory;
    private EntityCanvasAbilityIcon _entityCanvasAbilityIcon;
    private bool _abilityEnabled;
    
    public override void Enable()
    {
        EnableRerollAbility();
        
        _entityCanvasAbilityIcon = AddAbilityIcon(1);
    }

    public override void Disable()
    {
        DisableRerollAbility();
        
        RemoveAbilityIcon(_entityCanvasAbilityIcon);
    }
    
    private void EnableRerollAbility()
    {
        if (_abilityEnabled) return;
        
        Entity.Activated += TryActivateReroll;
        _abilityEnabled = true;
    }
    
    private void DisableRerollAbility()
    {
        if (!_abilityEnabled) return;
        
        Entity.Activated -= TryActivateReroll;
        _abilityEnabled = false;
    }

    private void TryActivateReroll()
    {
        if (_itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList().Count > 0)
        {
            RerollUnusedItems();
            
            Entity.Activated -= TryActivateReroll;
        }
    }

    private void RerollUnusedItems()
    {
        int destroyedItems = 0;
        int itemsTotalStrength = 0;

        List<Item> unusedItems = _itemFactory.CreatedItems.Except(_itemsContainer.ContainedItems).ToList();
        
        unusedItems.ForEach(unusedItem =>
        {
            //if (!unusedItem.EffectDatas.Exists(data => data.InstanceItemTypeContainers.Exists(typeContainer => typeContainer.InstanceType == typeof(StartWaveGlobalToggleEffect))))
            {
                destroyedItems++;
                itemsTotalStrength += unusedItem.Charges;
                _itemFactory.RemoveAndDestroyItem(unusedItem);
            }
        });

        _itemFactory.CreateItems(Entity.transform.position, itemsTotalStrength, 2, destroyedItems).Forget();
    }
}