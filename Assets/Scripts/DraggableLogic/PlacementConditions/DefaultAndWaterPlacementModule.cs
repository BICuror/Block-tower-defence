using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAndWaterPlacementModule", menuName = "DraggableSystem/PlacementConditions/DefaultAndWaterPlacementModule")]

public sealed class DefaultAndWaterPlacementModule : PlacementModule
{
    public override bool CanBePlaced(Vector2Int position)
    {
        int nonStackableTiels = TileMap.GetTileCount(position, LayerSettingType.SolidObjects);
    
        if (nonStackableTiels == 0) return true;
        
        if (nonStackableTiels == 1)
        {
            GameObject nonStackableTile = TileMap.GetHitObject(position, LayerSettingType.SolidObjects);

            if (nonStackableTile.TryGetComponent(out DraggableObject draggableObject))
            {
                return !draggableObject.IsPlaced;
            }
        }
        
        return false;
    }

    public override float GetHeight(Vector2Int position)
    {
        if (TileMap.HasTile(position, LayerSettingType.AnyTerrain))
        {
            RaycastHit hit = TileMap.GetHitInfo(position, LayerSettingType.AnyTerrain);
            
            return hit.point.y + AdditionalPlacementHeight;
        }

        return 1f + AdditionalPlacementHeight;
    }   

    public override Vector2Int GetPlacementPosition(Vector2Int position)
    {
        return position;
    }
}