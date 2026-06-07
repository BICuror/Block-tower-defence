using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using Combat;

public class InspectorController : MonoBehaviour
{
    [Inject] private TimeController _timeController;
    
    [SerializeField] private AreaVisualisationInspector _areaVisualisationInspector;
    [SerializeField] private InspectionTooltipManager _inspectionTooltipManager;
    
    private InspectableObject _inspectableObject;
    
    public event Action InspectionStopped;
    
    public bool TryToStartInspecting(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, LayerSettingType.InspectableObjects, out RaycastHit hit))
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
    

    public bool TryFindIdleInspectable(Vector2 mousePosition, out InspectableObject inspectableObject)
    {
        inspectableObject = null;
        
        if (_inspectionTooltipManager.NonIdleTooltipsOpened) return false;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (TileMap.HasTile(ray, LayerSettingType.InspectableObjects, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out InspectableObject hoveredInspectable))
            {
                inspectableObject = hoveredInspectable;
                return hoveredInspectable.CanBeIdleInspected;
            }
        }
        
        return false;
    }
    
    public bool HoveredOverInspectable(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (TileMap.HasTile(ray, LayerSettingType.InspectableObjects, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out InspectableObject hoveredInspectable))
            {
                return true;
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
        
        return !IsHoveredOverNonIdleUI();
    }

    public bool IsHoveredOverNonIdleUI() => _inspectionTooltipManager.HoveredOverNonIdleTooltip;
    
    public void StopInspecting() => _inspectionTooltipManager.DisableActiveSinglePopup();
    
    private async UniTask StartInspecting(InspectableObject inspectableObject)
    {
        if (inspectableObject.IsInspected) return;
        
        _inspectableObject = inspectableObject;
        inspectableObject.SetInspectedState(true);
        
        if (inspectableObject.PauseOnInspection) _timeController.Pause();

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
        
        if (inspectableObject.PauseOnInspection) _timeController.Resume();
        
        InspectionStopped?.Invoke();
    }
}