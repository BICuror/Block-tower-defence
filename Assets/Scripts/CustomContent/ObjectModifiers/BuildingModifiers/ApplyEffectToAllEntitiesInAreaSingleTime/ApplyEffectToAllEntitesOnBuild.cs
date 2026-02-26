using Cashing;

public class ApplyEffectToAllEntitesOnBuild : ApplyEffectOnceToEntitiesInArea
{
    [Cached] private BuildingDraggable _ownerBuildingDraggable;
    
    private void Start()
    {
        base.Start();
        _ownerBuildingDraggable.BuildCompleted += ApplyEffect;
    }
    
    private void OnDestroy()
    {
        _ownerBuildingDraggable.BuildCompleted -= ApplyEffect;
    }
}