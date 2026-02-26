public interface IDraggable 
{
    void PickUp();
    void Place();  
    void OnDrag();

    bool IsDraggable();   
    PlacementModule GetPlacementModule();
    DragAnimationObject GetDragAnimationObject();
}