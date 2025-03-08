public interface IDraggable 
{
    void PickUp();
    void Place();  

    bool IsDraggable();   
    PlacementModule GetPlacementModule();
    DragAnimationObject GetDragAnimationObject();
}