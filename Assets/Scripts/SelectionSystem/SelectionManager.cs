using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public sealed class SelectionManager : MonoBehaviour
{
    private Queue<SelectionSettings> _enqeuedSelections = new();
    private SelectionSettings _currentSelectionSettings;

    [SerializeField] private SelectionOptionObjectAreaDetector _selectionOptionObjectAreaDetector;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    
    [Header("Selectors")]
    [SerializeField] private BuildingSelector _buildingSelector;
    [SerializeField] private GlobalEffectSelector _globalEffectSelector;
    [SerializeField] private BuildingUpgradeSelector _buildingUpgradeSelector;
    
    private async void Start()
    {
        await UniTask.WaitForSeconds(1);
        
        StartSelection(new SelectionSettings(SelectionType.GlobalEffect));
        EnqeueSelection(new SelectionSettings(SelectionType.BuildingUpgrade));
        EnqeueSelection(new SelectionSettings(SelectionType.GlobalEffect));

        _selectionOptionObjectAreaDetector.AddedItem += ResolveCurrentSelection;
    }

    public void EnqeueSelection(SelectionSettings selectionSettings) => _enqeuedSelections.Enqueue(selectionSettings);

    
    
    public void StartSelection(SelectionSettings selectionSettings)
    {
        _currentSelectionSettings = selectionSettings;
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.Building: _buildingSelector.StartBuildingsSelection(); break;
            case SelectionType.GlobalEffect: _globalEffectSelector.StartGlobalEffectSelection(); break;
            case SelectionType.BuildingUpgrade: _buildingUpgradeSelector.StartGlobalEffectSelection(); break;
            default: throw new NotImplementedException($"Tried to start selection of type {_currentSelectionSettings.Type}");
        }
    }

    private void ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        optionObject.ApplyEffect();
        EndSelection(_currentSelectionSettings);
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        
        StartQueuedSelection();
    }
    private void StartQueuedSelection()
    {
        if (_enqeuedSelections.Count > 0)
        {
            SelectionSettings selectionSettings = _enqeuedSelections.Dequeue();
            StartSelection(selectionSettings);
        }
    }
    
    private void EndSelection(SelectionSettings selectionSettings)
    {
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.BuildingUpgrade: _buildingUpgradeSelector.EndSelection(); break;
            default: break;
        }
    }
    
    public bool SelectionOptionCanBePlaced(SelectionType type)
    {
        return type == _currentSelectionSettings.Type;
    }
}