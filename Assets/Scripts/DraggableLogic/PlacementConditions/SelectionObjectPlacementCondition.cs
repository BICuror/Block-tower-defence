using UnityEngine;
using Combat;

[CreateAssetMenu(fileName = "SelectionObjectPlacementCondition", menuName = "PlacementConditions/SelectionObjectPlacementCondition")]

public sealed class SelectionObjectPlacementCondition : PlacementModule
{
    [SerializeField] private SelectionType _selectionType;
    [SerializeField] private LayerSetting _sutableTerrainLayerSetting;
    [SerializeField] private LayerSetting _nonStackableLayerSetting;
    [SerializeField] private LayerSetting _townhallLayerSetting;

    public override bool CanBePlaced(Vector2Int position)
    {
        if (TileMap.HasTile(position, _townhallLayerSetting, out RaycastHit hit))
        {
            SelectionManager selectionManager = hit.collider.transform.parent.gameObject.GetComponent<BuildingEntity>().ComponentsContainer.Get<SelectionManager>();

            return selectionManager.SelectionOptionCanBePlaced(_selectionType);
        }
        
        if (!TileMap.HasTile(position, _sutableTerrainLayerSetting)) return false;
        
        int nonStackableTiles = TileMap.GetTileCount(position, _nonStackableLayerSetting);

        if (nonStackableTiles == 0) return true;
        if (nonStackableTiles == 1)
        {
            GameObject nonStackableTile = TileMap.GetHitObject(position, _nonStackableLayerSetting);

            if (nonStackableTile.TryGetComponent(out DraggableObject draggableObject))
            {
                return !draggableObject.IsPlaced();
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