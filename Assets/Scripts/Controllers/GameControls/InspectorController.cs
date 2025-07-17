using UnityEngine;
using Combat;

[RequireComponent(typeof(Camera))]

public class InspectorController : MonoBehaviour
{
    [SerializeField] private InspectionTooltipManager _inspectionTooltipManager;
    [SerializeField] private AreaVisualisation _areaVisualisation;
    [SerializeField] private LayerSetting _inspectableLayerSetting;
    
    private Inspectable _inspectable;
    
    public bool TryToStartInspecting(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out Inspectable hoveredInspectable))
            { 
                StartInspecting(hoveredInspectable);
    
                return true;
            }
        }

        return false;
    }
    
    public bool TryToStartInspectingItem(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out Item item))
            {
                _inspectable = hit.collider.gameObject.GetComponent<Inspectable>();
                
                _inspectable.SetInspectedState(true);
                
                _inspectionTooltipManager.ActivateCrystalTooltip(item);
                
                return true;
            }
        }

        return false;
    }
    
    public bool TryToStartInspectingEffect(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out BuildingUpgradeSelectionOptionObject buildingUpgradeSelectionOptionObject))
            {
                _inspectable = hit.collider.gameObject.GetComponent<Inspectable>();
                
                _inspectable.SetInspectedState(true);
                
                _inspectionTooltipManager.ActivateEffectTooltip(buildingUpgradeSelectionOptionObject);
                
                return true;
            }
        }

        return false;
    }

    public bool TryStopInspecting(Vector2 mousePosition)
    {
        if (_inspectable != null && _inspectable.TryGetComponent<Item>(out Item inspectedItem))
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out Item item))
                {
                    if (inspectedItem != item)
                    {
                        StopInspecting();
                        TryToStartInspectingItem(mousePosition);
                        return false;
                    }
                }
            }
        }
        
        if (_inspectable != null && _inspectable.TryGetComponent<BuildingUpgradeSelectionOptionObject>(out BuildingUpgradeSelectionOptionObject selectionOptionObjectInspectable))
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out BuildingUpgradeSelectionOptionObject selectionOptionObject))
                {
                    if (selectionOptionObjectInspectable != selectionOptionObject)
                    {
                        StopInspecting();
                        TryToStartInspectingEffect(mousePosition);
                        return false;
                    }
                }
            }
        }
        
        if (_inspectionTooltipManager.NonIdleTooltipsOpened) return false;
        
        StopInspecting();

        return true;
    }

    private void StartInspecting(Inspectable inspectable)
    {
        _inspectable = inspectable;
        _inspectable.SetInspectedState(true);
        
        _areaVisualisation.ActivateVisualisation(_inspectable.gameObject);
        
        if (inspectable.TryGetComponent(out BuildingSelectionOptionObject buildingOptionObject))
        {
            _inspectionTooltipManager.ActivateEntityTooltip(buildingOptionObject.InstantiatedBuilding);
        }
        else if (inspectable.TryGetComponent(out CombatEntity combatEntity))
        {
            _inspectionTooltipManager.ActivateEntityTooltip(combatEntity);
        }
    }

    public void StopInspecting()
    {
        if (!_inspectable) return;
        
        _inspectionTooltipManager.CloseAllTooltips();
        _areaVisualisation.DeactivateVisualisation(_inspectable.gameObject);
        _inspectable.SetInspectedState(false);
        _inspectable = null;
    }
}