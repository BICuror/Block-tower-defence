using Zenject;

public sealed class BuildingsExsistEffectApperanceCondition : EffectApperanceCondition
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    
    public override bool CanAppear()
    {
        return _globalBuildingContainer.Entities.Count > 0;
    }
}