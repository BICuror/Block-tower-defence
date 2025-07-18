using Zenject;

public sealed class MaxBuildingsAmountApperanceCondition : EffectApperanceCondition
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private GlobalStatContainer _globalStatContainer;

    public override bool CanAppear()
    {
        return _globalBuildingContainer.Entities.Count < _globalStatContainer.Get<MaxBuildings>().RoundedValue;
    }
}