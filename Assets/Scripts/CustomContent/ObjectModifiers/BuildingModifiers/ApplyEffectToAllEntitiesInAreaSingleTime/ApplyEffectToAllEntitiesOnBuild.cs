using Cashing;

public class ApplyEffectToAllEntitiesOnBuild : ApplyEffectOnceToEntitiesInArea
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