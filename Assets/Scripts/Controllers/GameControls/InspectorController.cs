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
    
    private Inspectable _inspectable;
    
    public Action InspectionStopped;
    
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
        
        _inspectionTooltipManager.SetActiveLayer(UILayer.Group);

        return false;
    }
    
    public bool TryToStartIdleInspecting(Vector2 mousePosition)
    {
        if (_inspectionTooltipManager.NonIdleTooltipsOpened) return false;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out Inspectable hoveredInspectable))
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
        if (_inspectable != null && draggedObject == _inspectable.gameObject)
        {
            return true;
        }
        
        return !_inspectionTooltipManager.HoveredOverNonIdleTooltip;
    }
    
    public void StopInspecting()
    {
        _inspectionTooltipManager.DisableActiveSinglePopup();
    }
    
    private async UniTask StartInspecting(Inspectable inspectable)
    {
        if (_inspectable && _inspectable == inspectable) return;
        if (inspectable.IsInspected) return;
        
        _inspectable = inspectable;
        _inspectable.SetInspectedState(true);

        _areaVisualisationInspector.ActivateVisualisation(_inspectable.gameObject);
        
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
            await _inspectionTooltipManager.OpenEffectTooltip(selectionOptionObject.ModificatorData, selectionOptionObject.transform);
        }

        if (inspectable)
        {
            _areaVisualisationInspector.DeactivateVisualisation(inspectable.gameObject);
            inspectable.SetInspectedState(false);
        }
        
        _inspectable = null;
        InspectionStopped?.Invoke();
    }
}