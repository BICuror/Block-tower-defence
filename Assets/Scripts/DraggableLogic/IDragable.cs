public interface IDraggable 
{
    void PickUp();
    
    void Place();  

    bool IsDraggable();   

    PlacementCondition GetPlacementCondition();
}