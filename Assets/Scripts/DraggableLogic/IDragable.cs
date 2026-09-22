public interface IDraggable 
{
    int TileScale { get; }
    
    void PickUp();
    void Place();  
    void OnDrag();

    bool CanBeDragged();   
    PlacementModule GetPlacementModule();
    DragAnimationObject GetDragAnimationObject();
}