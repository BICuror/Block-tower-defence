using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlacementModule", menuName = "DraggableSystem/PlacementConditions/DefaultPlacementModule")]

public sealed class DefaultPlacementModule : PlacementModule
{
    [SerializeField] private LayerSettingType _sutableTerrainLayer = LayerSettingType.SolidTerrain;

    public override bool CanBePlaced(Vector2Int position)
    {
        if (TileMap.HasTile(position, _sutableTerrainLayer))
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
        }

        return false;
    }

    public override float GetHeight(Vector2Int position)
    {
        RaycastHit hit = TileMap.GetHitInfo(position, _sutableTerrainLayer);
        
        return hit.point.y + AdditionalPlacementHeight;
    }

    public override Vector2Int GetPlacementPosition(Vector2Int position)
    {
        return position;
    }
}