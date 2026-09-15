using NaughtyAttributes;
using UnityEngine;
using Combat;

[CreateAssetMenu(fileName = "SelectionObjectPlacementCondition", menuName = "DraggableSystem/PlacementConditions/SelectionObjectPlacementCondition")]

public sealed class SelectionObjectPlacementCondition : PlacementModule
{
    [SerializeField] private bool _requiresSelectionType = true;
    [ShowIf("_requiresSelectionType")] [SerializeField] private SelectionType _selectionType;
    
    public override bool CanBePlaced(Vector2 position, int tileScale)
    {
        Vector2Int roundPosition = position.ToIntVector();

        if (TileMap.HasTile(roundPosition, LayerSettingType.Townhall, out RaycastHit hit))
        {
            SelectionManager selectionManager = hit.collider.transform.parent.gameObject.GetComponent<BuildingEntity>().ComponentsContainer.Get<SelectionManager>();

            return selectionManager.SelectionOptionsCanBePlaced && (!_requiresSelectionType || selectionManager.CurrentSelection.SelectionType == _selectionType);
        }
        
        if (!IsValidPosition(roundPosition)) return false;
        
        return TileMap.DraggableCanBePlacedAccordingToScale(position, tileScale, LayerSettingType.AnyTerrain, LayerSettingType.NonstackableCreatedItems);
    }

    public override float GetHeight(Vector2 position)
    {
        Vector2Int roundPosition = position.ToIntVector();
        
        if (TileMap.HasTile(roundPosition, LayerSettingType.Townhall, out RaycastHit hit))
        {
            return hit.point.y;
        }
        
        float height = TileMap.GetHitInfo(roundPosition, LayerSettingType.SolidTerrain).point.y;

        if (height < 1) height = 1;

        return height + AdditionalPlacementHeight;
    }
    
    public override Vector2 GetPlacementPosition(Vector2 position, int tileScale)
    {
        Vector2Int roundPosition = position.ToIntVector();
        
        if (TileMap.HasTile(roundPosition, LayerSettingType.Townhall))
        {
            GameObject townhall = TileMap.GetHitObject(roundPosition, LayerSettingType.Townhall);
            
            return new Vector2Int(Mathf.RoundToInt(townhall.transform.position.x), Mathf.RoundToInt(townhall.transform.position.z)) - TileMap.GetTileSizeDraggableObjectOffset(tileScale);
        }

        return position;
    }
}