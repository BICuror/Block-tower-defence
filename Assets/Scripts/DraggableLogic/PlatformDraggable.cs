public sealed class PlatformDraggable : DraggableObject
{
    public override bool CanBeDragged() => !TileMap.HasObjectsInTileScale(transform.position.ToVector2(), TileScale, LayerSettingType.SolidObjects) && DraggableState == DraggableState.Placed;
}