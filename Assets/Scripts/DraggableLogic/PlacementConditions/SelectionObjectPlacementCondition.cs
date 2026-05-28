using UnityEngine;
using Combat;

[CreateAssetMenu(fileName = "SelectionObjectPlacementCondition", menuName = "DraggableSystem/PlacementConditions/SelectionObjectPlacementCondition")]

public sealed class SelectionObjectPlacementCondition : PlacementModule
{
    [SerializeField] private SelectionType _selectionType;
    
    public override bool CanBePlaced(Vector2Int position)
    {
        if (TileMap.HasTile(position, LayerSettingType.Townhall, out RaycastHit hit))
        {
            SelectionManager selectionManager = hit.collider.transform.parent.gameObject.GetComponent<BuildingEntity>().ComponentsContainer.Get<SelectionManager>();

            return selectionManager.SelectionOptionCanBePlaced(_selectionType);
        }
        
        if (!TileMap.HasTile(position, LayerSettingType.SolidTerrain)) return false;
        
        int nonStackableTiles = TileMap.GetTileCount(position, LayerSettingType.SolidObjects);

        if (nonStackableTiles == 0) return true;
        if (nonStackableTiles == 1)
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