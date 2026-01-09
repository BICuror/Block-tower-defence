using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

public sealed class SelectionManager : MonoBehaviour
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;

    [SerializeField] private SelectionOptionObjectAreaDetector _selectionOptionObjectAreaDetector;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    
    [Header("Selectors")]
    [SerializeField] private BuildingSelector _buildingSelector;
    [SerializeField] private BuildingUpgradeSelector _buildingUpgradeSelector;
    
    private Queue<SelectionSettings> _enqeuedSelections = new();
    private SelectionSettings _currentSelectionSettings;
    private bool _selectionOptionsCanBePlaced;
    private bool _selectionIsActive;
    
    public bool SelectionPhaseIsActive => _selectionIsActive;
    public SelectionType SelectionType => _currentSelectionSettings.Type;

    public event Action SelectionStarted; 
    public event Action SelectionEnded;
    public event Action SelectionStepStarted;
    public event Action SelectionStepEnded;
    
    private async void Start()
    {
        _selectionOptionObjectAreaDetector.AddedItem += (optionObject) => ResolveCurrentSelection(optionObject).Forget();

        await UniTask.WaitForSeconds(5);
    }
    
    public bool SelectionOptionCanBePlaced(SelectionType type)
    {
        return _selectionOptionsCanBePlaced && type == _currentSelectionSettings.Type;
    }

    public bool TryEnqueueNewBuildingSelection()
    {
        if (_globalBuildingContainer.Entities.Count < _globalStatContainer.Get<MaxBuildings>().Value)
        {
            EnqueueSelection(new SelectionSettings(SelectionType.Building));
            EnqueueSelection(new SelectionSettings(SelectionType.BuildingUpgrade, true));
        }

        return false;
    }
    
    public void EnqueueSelection(SelectionSettings selectionSettings) => _enqeuedSelections.Enqueue(selectionSettings);
    
    public void TryStartQueuedSelection()
    {
        if (_enqeuedSelections.Count > 0 && !_selectionIsActive)
        {
            _selectionIsActive = true;
            
            StartQueuedSelection().Forget();
            
            SelectionStarted?.Invoke();
        }
    }
    
    private async UniTask StartQueuedSelection()
    {
        _currentSelectionSettings = _enqeuedSelections.Dequeue();
        
        SelectionStepStarted?.Invoke(); 
        
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.Building:
            {
                if (_globalBuildingContainer.Entities.Count < _globalStatContainer.Get<MaxBuildings>().Value)
                {
                    await _buildingSelector.StartBuildingsSelection(_currentSelectionSettings);
                }
                break;
            }
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.StartUpgradeSelection(_currentSelectionSettings); break;
            default: throw new NotImplementedException($"Tried to start selection of type {_currentSelectionSettings.Type}");
        }
        
        _selectionOptionsCanBePlaced = true;
    }

    private async UniTask ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        optionObject.ApplyEffect();
        
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        _selectionOptionsCanBePlaced = false;
        
        SelectionStepEnded?.Invoke(); 
        
        await EndSelection(_currentSelectionSettings);

        if (_enqeuedSelections.Count > 0) StartQueuedSelection().Forget();
        else
        {
            _selectionIsActive = false;
            
            SelectionEnded?.Invoke();
        }
    }
    
    private async UniTask EndSelection(SelectionSettings selectionSettings)
    {
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.EndSelection(); break;
            case SelectionType.Building: await UniTask.WaitForSeconds(1f); break;
        }
    }
}