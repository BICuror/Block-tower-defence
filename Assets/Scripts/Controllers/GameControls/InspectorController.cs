using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using Combat;
using UnityEngine.Serialization;

[RequireComponent(typeof(Camera))]

public class InspectorController : MonoBehaviour
{
    [SerializeField] private InspectionTooltipManager _inspectionTooltipManager;
    [FormerlySerializedAs("_areaVisualisation")] [SerializeField] private AreaVisualisationInspector _areaVisualisationInspector;
    [SerializeField] private LayerSetting _inspectableLayerSetting;
    
    private InspectableObject _inspectableObject;
    
    public Action InspectionStopped;
    
    public bool TryToStartInspecting(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out InspectableObject hoveredInspectable))
            { 
                StartInspecting(hoveredInspectable);
                
                return true;
            }
        }

        StopInspecting();

        return false;
    }
    
    public bool TryToStartIdleInspecting(Vector2 mousePosition)
    {
        if (_inspectionTooltipManager.NonIdleTooltipsOpened) return false;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out InspectableObject hoveredInspectable))
            {
                if (hoveredInspectable.CanBeIdleInspected)
                {
                    StartInspecting(hoveredInspectable);

                    return true;
                }
            }
        }
        
        return false;
    }

    public bool IsPossibleToDragInspectedItem(GameObject draggedObject)
    {
        if (_inspectableObject != null && draggedObject == _inspectableObject.gameObject)
        {
            return true;
        }
        
        return !_inspectionTooltipManager.HoveredOverNonIdleTooltip;
    }
    
    public void StopInspecting() => _inspectionTooltipManager.DisableActiveSinglePopup();
    
    private async UniTask StartInspecting(InspectableObject inspectableObject)
    {
        if (inspectableObject.IsInspected) return;
        
        _inspectableObject = inspectableObject;
        inspectableObject.SetInspectedState(true);

        _areaVisualisationInspector.ActivateVisualisation(inspectableObject.gameObject);
        
        _inspectionTooltipManager.DisableActiveSinglePopup();
        
        if (inspectableObject.TryGetComponent(out BuildingSelectionOptionObject buildingOptionObject))
        {
            await _inspectionTooltipManager.OpenEntityTooltip(buildingOptionObject.InstantiatedBuilding);
        }
        else if (inspectableObject.TryGetComponent(out CombatEntity combatEntity))
        {
            await _inspectionTooltipManager.OpenEntityTooltip(combatEntity);
        }
        else if (inspectableObject.TryGetComponent(out Item item))
        {
            await _inspectionTooltipManager.OpenCrystalTooltip(item);
        }        
        else if (inspectableObject.TryGetComponent(out BuildingUpgradeSelectionOptionObject selectionOptionObject))
        {
            await _inspectionTooltipManager.OpenEffectTooltip(selectionOptionObject.ModificatorData, selectionOptionObject.transform);
        }
        
        if (inspectableObject)
        {
            _areaVisualisationInspector.DeactivateVisualisation(inspectableObject.gameObject);
            inspectableObject.SetInspectedState(false);
        }
        
        InspectionStopped?.Invoke();
    }
}