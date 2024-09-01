public interface IDraggable 
{
    void PickUp();
    
    void Place();  

    bool IsDraggable();   

    PlacementRequirements GetPlacementRequirements();

    bool CanBePlacedAt(float x, float z, LayerSetting layerSetting);
}

public enum PlacementRequirements
{
    SolidSurface,
    TransitionSurface,
    Any
}
