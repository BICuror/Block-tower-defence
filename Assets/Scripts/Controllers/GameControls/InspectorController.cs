using UnityEngine;
using Combat;

[RequireComponent(typeof(Camera))]

public class InspectorController : MonoBehaviour
{
    [SerializeField] private InspectionTooltipManager _inspectionTooltipManager;
    [SerializeField] private AreaVisualisation _areaVisualisation;
    [SerializeField] private LayerSetting _inspectableLayerSetting;
    
    private Inspectable _inspectable;
    
    public void TryToStartInspecting(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out Inspectable hoveredInspectable))
            { 
                StartInspecting(hoveredInspectable);
                
                return;
            }
        }
        
        _inspectionTooltipManager.SetActiveLayer(UILayer.Group);
    }
    
    public void TryToStartIdleInspecting(Vector2 mousePosition)
    {
        if (_inspectionTooltipManager.NonIdleTooltipsOpened) return;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out Inspectable hoveredInspectable))
            {
                if (hoveredInspectable.CanBeIdleInspected)
                {
                    StartInspecting(hoveredInspectable);
                }
            }
        }
    }
    
    
    private async void StartInspecting(Inspectable inspectable)
    {
        if (_inspectable && _inspectable == inspectable) return;
        if (inspectable.IsInspected) return;
        
        _inspectable = inspectable;
        _inspectable.SetInspectedState(true);
        
        _areaVisualisation.ActivateVisualisation(_inspectable.gameObject);
        
        _inspectionTooltipManager.DisableActiveSinglePopup();
        
        if (inspectable.TryGetComponent(out BuildingSelectionOptionObject buildingOptionObject))
        {
            await _inspectionTooltipManager.OpenEntityTooltip(buildingOptionObject.InstantiatedBuilding);
        }
        else if (inspectable.TryGetComponent(out CombatEntity combatEntity))
        {
            await _inspectionTooltipManager.OpenEntityTooltip(combatEntity);
        }
        else if (inspectable.TryGetComponent(out Item item))
        {
            await _inspectionTooltipManager.OpenCrystalTooltip(item);
        }        
        else if (inspectable.TryGetComponent(out BuildingUpgradeSelectionOptionObject selectionOptionObject))
        {
            await _inspectionTooltipManager.OpenEffectTooltip(selectionOptionObject);
        }

        if (inspectable)
        {
            _areaVisualisation.DeactivateVisualisation(inspectable.gameObject);
            inspectable.SetInspectedState(false);
        }
        
        _inspectable = null;
    }
}