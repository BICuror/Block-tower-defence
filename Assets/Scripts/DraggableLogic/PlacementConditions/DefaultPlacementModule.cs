using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlacementModule", menuName = "DraggableSystem/PlacementConditions/DefaultPlacementModule")]

public sealed class DefaultPlacementModule : PlacementModule
{
    [SerializeField] private LayerSettingType _sutableTerrainLayer = LayerSettingType.SolidTerrain;

    public override bool CanBePlaced(Vector2 position, int tileScale)
    {
        return TileMap.DraggableCanBePlacedAccordingToScale(position, tileScale, _sutableTerrainLayer, LayerSettingType.SolidObjects);
    }

    public override float GetHeight(Vector2 position)
    {
        RaycastHit hit = TileMap.GetHitInfo(position.ToIntVector(), _sutableTerrainLayer);
        
        return hit.point.y + AdditionalPlacementHeight;
    }

    public override Vector2 GetPlacementPosition(Vector2 position, int tileScale)
    {
        return position;
    }
}