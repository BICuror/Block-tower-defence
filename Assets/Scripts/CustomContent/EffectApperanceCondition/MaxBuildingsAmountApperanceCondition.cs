using Combat.Stats;
using Configs;
using Zenject;

namespace ScriptableLogic.Conditions
{
    public sealed class MaxBuildingsAmountApperanceCondition : Condition, ICondition
    {
        [Argument("TrueWhenFull")] private bool _trueWhenFull;
        [Inject] private GlobalBuildingContainer _globalBuildingContainer;
        [Inject] private GlobalStatContainer _globalStatContainer;
        
        public bool GetValue()
        {
            bool isMaxBuildingsReached = _globalBuildingContainer.Entities.Count >= _globalStatContainer.Get<MaxBuildings>().RoundedValue;
            
            return isMaxBuildingsReached == _trueWhenFull;
        }
    }
}