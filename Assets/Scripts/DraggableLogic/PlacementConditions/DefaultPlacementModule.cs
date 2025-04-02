using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlacementModule", menuName = "PlacementConditions/DefaultPlacementModule")]

public sealed class DefaultPlacementModule : PlacementModule
{
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;

    public override bool CanBePlaced(Vector2Int position)
    {
        if (TileMap.HasTile(position, _sutableTerrainLayerSetting))
        {
            int nonStackableTiels = TileMap.GetTileCount(position, _nonStackableLayerSetting);
    
            if (nonStackableTiels == 0) return true;
            if (nonStackableTiels == 1)
            {
                GameObject nonStackableTile = TileMap.GetHitObject(position, _nonStackableLayerSetting);
    
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
        RaycastHit hit = TileMap.GetHitInfo(position, _sutableTerrainLayerSetting);
        
        return hit.point.y + AdditionalPlacementHeight;
    }

    public override Vector2Int GetPlacementPosition(Vector2Int position)
    {
        return position;
    }
}