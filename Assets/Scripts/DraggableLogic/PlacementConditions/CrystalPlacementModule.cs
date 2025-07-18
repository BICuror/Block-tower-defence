using Combat;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalPlacementModule", menuName = "PlacementConditions/CrystalPlacementModule")]

public sealed class CrystalPlacementModule : PlacementModule
{
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;
    [SerializeField] private LayerSetting _townhallLayerSetting;

    public override bool CanBePlaced(Vector2Int position)
    {
        if (TileMap.HasTile(position, _townhallLayerSetting))
        {
            GameObject townhall = TileMap.GetHitObject(position, _townhallLayerSetting).transform.parent.gameObject;
            
            return townhall.GetComponentInChildren<SelectionManager>().SelectionPhaseIsActive == false;
        }
        
        if (!TileMap.HasTile(position, _sutableTerrainLayerSetting)) return false;
        
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

        return false; 
    }

    public override float GetHeight(Vector2Int position)
    {
        if (TileMap.HasTile(position, _townhallLayerSetting, out RaycastHit hit))
        {
            return hit.point.y;
        }
        
        return TileMap.GetHitInfo(position, _sutableTerrainLayerSetting).point.y + AdditionalPlacementHeight;
    }
    
    public override Vector2Int GetPlacementPosition(Vector2Int position)
    {
        if (TileMap.HasTile(position, _townhallLayerSetting))
        {
            GameObject townhall = TileMap.GetHitObject(position, _townhallLayerSetting);
            
            return new Vector2Int(Mathf.RoundToInt(townhall.transform.position.x), Mathf.RoundToInt(townhall.transform.position.z));
        }

        return position;
    }
}