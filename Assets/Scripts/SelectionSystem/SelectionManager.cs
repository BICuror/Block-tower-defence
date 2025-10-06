using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using Zenject;

public sealed class SelectionManager : MonoBehaviour
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    private Queue<SelectionSettings> _enqeuedSelections = new();
    private SelectionSettings _currentSelectionSettings;

    [SerializeField] private SelectionOptionObjectAreaDetector _selectionOptionObjectAreaDetector;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    
    [Header("Selectors")]
    [SerializeField] private BuildingSelector _buildingSelector;
    [SerializeField] private GlobalEffectSelector _globalEffectSelector;
    [SerializeField] private BuildingUpgradeSelector _buildingUpgradeSelector;
    private bool _selectionIsActive;
    
    public bool SelectionPhaseIsActive => _enqeuedSelections.Count != 0 || _selectionIsActive;
    
    private async void Start()
    {
        _selectionOptionObjectAreaDetector.AddedItem += (optionObject) => ResolveCurrentSelection(optionObject).Forget();
        
        EnqeueSelection(new SelectionSettings(SelectionType.Building));
        EnqeueSelection(new SelectionSettings(SelectionType.BuildingUpgrade));

        await UniTask.WaitForSeconds(5);
    }
    
    public void EnqeueSelection(SelectionSettings selectionSettings) => _enqeuedSelections.Enqueue(selectionSettings);
    
    public async UniTask StartSelection(SelectionSettings selectionSettings)
    {
        _currentSelectionSettings = selectionSettings;
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.Building:
            {
                if (_globalBuildingContainer.Entities.Count >= _globalStatContainer.Get<MaxBuildings>().Value)
                {
                    _currentSelectionSettings.Type = SelectionType.BuildingUpgrade;
                    await _buildingUpgradeSelector.StartUpgradeSelection(selectionSettings);
                }
                else await _buildingSelector.StartBuildingsSelection(); break;
            }
            case SelectionType.GlobalEffect: await _globalEffectSelector.StartGlobalEffectSelection(); break;
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.StartUpgradeSelection(selectionSettings); break;
            default: throw new NotImplementedException($"Tried to start selection of type {_currentSelectionSettings.Type}");
        }

        _selectionIsActive = true;
    }

    private async UniTask ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        optionObject.ApplyEffect();
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        await EndSelection(_currentSelectionSettings);
        
        TryStartQueuedSelection().Forget();
    }
    
    public async UniTask TryStartQueuedSelection()
    {
        if (_enqeuedSelections.Count > 0)
        {
            SelectionSettings selectionSettings = _enqeuedSelections.Dequeue();
            await StartSelection(selectionSettings);
        }
    }
    
    private async UniTask EndSelection(SelectionSettings selectionSettings)
    {
        _selectionIsActive = false;
        
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.EndSelection(); break;
            case SelectionType.Building: await UniTask.WaitForSeconds(1f); break;
            default: break;
        }
    }
    
    public bool SelectionOptionCanBePlaced(SelectionType type)
    {
        return _selectionIsActive && type == _currentSelectionSettings.Type;
    }
}