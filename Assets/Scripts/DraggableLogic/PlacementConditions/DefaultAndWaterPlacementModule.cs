using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAndWaterPlacementModule", menuName = "DraggableSystem/PlacementConditions/DefaultAndWaterPlacementModule")]

public sealed class DefaultAndWaterPlacementModule : PlacementModule
{
    public override bool CanBePlaced(Vector2 position, int tileScale)
    {
        return TileMap.DraggableCanBePlacedAccordingToScale(position, tileScale, LayerSettingType.WaterAndTerrain, LayerSettingType.SolidObjects);
    }

    public override float GetHeight(Vector2 position)
    {
        Vector2Int roundedPosition = Vector2Int.RoundToInt(position);
        
        if (TileMap.HasTile(roundedPosition, LayerSettingType.AnyTerrain))
        {
            RaycastHit hit = TileMap.GetHitInfo(roundedPosition, LayerSettingType.AnyTerrain);
            
            return hit.point.y + AdditionalPlacementHeight;
        }

        return 1f + AdditionalPlacementHeight;
    }   

    public override Vector2 GetPlacementPosition(Vector2 position, int tileScale)
    {
        return position;
    }
}