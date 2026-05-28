using UnityEngine;

[CreateAssetMenu(fileName = "CrystalPlacementModule", menuName = "DraggableSystem/PlacementConditions/CrystalPlacementModule")]

public sealed class CrystalPlacementModule : PlacementModule
{
    public override bool CanBePlaced(Vector2Int position)
    {
        if (TileMap.HasTile(position, LayerSettingType.Townhall))
        {
            GameObject townhall = TileMap.GetHitObject(position, LayerSettingType.Townhall).transform.parent.gameObject;
            
            return townhall.GetComponentInChildren<SelectionManager>().SelectionPhaseIsActive == false;
        }
        
        if (!TileMap.HasTile(position, LayerSettingType.SolidTerrain)) return false;
        
        int nonStackableTiels = TileMap.GetTileCount(position, LayerSettingType.NonstackableCreatedItems);

        if (nonStackableTiels == 0) return true;
        if (nonStackableTiels == 1)
        {
            GameObject nonStackableTile = TileMap.GetHitObject(position, LayerSettingType.NonstackableCreatedItems);

            if (nonStackableTile.TryGetComponent(out DraggableObject draggableObject))
            {
                return !draggableObject.IsPlaced;
            }
        }

        return false; 
    }

    public override float GetHeight(Vector2Int position)
    {
        if (TileMap.HasTile(position, LayerSettingType.Townhall, out RaycastHit hit))
        {
            return hit.point.y;
        }
        
        return TileMap.GetHitInfo(position, LayerSettingType.SolidTerrain).point.y + AdditionalPlacementHeight;
    }
    
    public override Vector2Int GetPlacementPosition(Vector2Int position)
    {
        if (TileMap.HasTile(position, LayerSettingType.Townhall))
        {
            GameObject townhall = TileMap.GetHitObject(position, LayerSettingType.Townhall);
            
            return new Vector2Int(Mathf.RoundToInt(townhall.transform.position.x), Mathf.RoundToInt(townhall.transform.position.z));
        }

        return position;
    }
}