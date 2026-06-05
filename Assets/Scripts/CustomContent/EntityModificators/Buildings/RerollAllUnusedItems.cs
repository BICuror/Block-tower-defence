using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        _waveStateMachine.StateStarted += UpdateAbilityState;
        EnableRerollAbility();
        
        _entityCanvasAbilityIcon = AddAbilityIcon(1);
        _entityCanvasAbilityIcon.SetActiveWaveState(WaveState.Idle);
    }

    public override void Disable()
    {
        _waveStateMachine.StateStarted -= UpdateAbilityState;
        DisableRerollAbility();
        
        RemoveAbilityIcon(_entityCanvasAbilityIcon);
    }

    private void UpdateAbilityState(WaveState waveState)
    {
        if (waveState == WaveState.Attack)
        {
            DisableRerollAbility();
            _entityCanvasAbilityIcon.SetValue(0).Forget();
        }
        else if (waveState == WaveState.Idle)
        {
            EnableRerollAbility();
            _entityCanvasAbilityIcon.SetValue(1).Forget();
        }
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
            
            _entityCanvasAbilityIcon.SetValue(0).Forget();
            
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
            if (!unusedItem.EffectDatas.Exists(data => data.InstanceItemTypeContainers.Exists(typeContainer => typeContainer.InstanceType == typeof(StartWaveGlobalToggleEffect))))
            {
                destroyedItems++;
                itemsTotalStrength += unusedItem.Charges;
                _itemFactory.RemoveAndDestroyItem(unusedItem);
            }
        });

        _itemFactory.CreateItems(Entity.transform.position, itemsTotalStrength, 2, destroyedItems).Forget();
    }
}