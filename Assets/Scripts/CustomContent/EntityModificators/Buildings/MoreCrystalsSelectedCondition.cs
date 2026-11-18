using Combat.Items;
using Configs;
using Zenject;

namespace ScriptableLogic.Conditions
{
    public sealed class MoreCrystalsSelectedCondition : Condition, ICondition
    {
        [Argument("ItemThresholdAmount")] private int _itemThresholdAmount;
        [Inject] private ItemsContainer _itemsContainer;
        
        public bool GetValue() => _itemsContainer.ContainedItems.Count >= _itemThresholdAmount;
    }
    
    public sealed class LessCrystalsSelectedCondition : Condition, ICondition
    {
        [Argument("ItemThresholdAmount")] private int _itemThresholdAmount;
        [Inject] private ItemsContainer _itemsContainer;
        
        public bool GetValue() => _itemsContainer.ContainedItems.Count <= _itemThresholdAmount;
    }
}