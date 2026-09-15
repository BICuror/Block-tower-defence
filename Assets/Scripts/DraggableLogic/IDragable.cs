public interface IDraggable 
{
    int TileScale { get; }
    
    void PickUp();
    void Place();  
    void OnDrag();

    bool IsDraggable();   
    PlacementModule GetPlacementModule();
    DragAnimationObject GetDragAnimationObject();
}