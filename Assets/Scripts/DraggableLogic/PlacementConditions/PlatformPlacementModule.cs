using UnityEngine;

[CreateAssetMenu(fileName = "PlatformPlacementModule", menuName = "DraggableSystem/PlacementConditions/PlatformPlacementModule")]

public sealed class PlatformPlacementModule : PlacementModule
{
    [SerializeField] private LayerSettingType _sutableTerrainLayer = LayerSettingType.WaterAndAnyTerrainWithoutPlatforms;

    public override bool CanBePlaced(Vector2 position, int tileScale)
    {
        Vector2 offset = new Vector2((tileScale - 1) * -0.5f, (tileScale - 1) * -0.5f);
    
        for (int x = 0; x < tileScale; x++)
        {
            for (int z = 0; z < tileScale; z++)
            {
                Vector2Int checkPosition = new Vector2(x + offset.x + position.x, z + offset.y + position.y).ToIntVector();
    
                if (TileMap.HasTileIgnoringDraggedObjects(checkPosition, LayerSettingType.SolidObjects)) return false;
                if (TileMap.HasTileIgnoringDraggedObjects(checkPosition, LayerSettingType.AnyTerrain)) return false;
            }
        }

        return true;
    }

    public override float GetHeight(Vector2 position, int tileScale)
    {
        float height = int.MinValue;
    
        Vector2 offset = new Vector2((tileScale - 1) * -0.5f, (tileScale - 1) * -0.5f);

        for (int x = 0; x < tileScale; x++)
        {
            for (int z = 0; z < tileScale; z++)
            {
                Vector2Int checkPosition = new Vector2(x + offset.x + position.x, z + offset.y + position.y).ToIntVector();

                RaycastHit hit = TileMap.GetHitInfo(checkPosition, _sutableTerrainLayer);

                if (hit.point.y > height)
                {
                    height = hit.point.y;
                }
            }
        }

        height += AdditionalPlacementHeight;

        if (height < 1f) height = 1f;

        return height;
    }

    public override Vector2 GetPlacementPosition(Vector2 position, int tileScale)
    {
        return position;
    }
}