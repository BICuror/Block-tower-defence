using Zenject;

public sealed class BuildingsAmountApperanceCondition : EffectApperanceCondition
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;

    public override bool GetValue()
    {
        return _globalBuildingContainer.Entities.Count < Args.GetArgument<int>("MaxBuildngsAmount");
    }
}