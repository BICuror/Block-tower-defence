using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using CuroAudio;
using NaughtyAttributes;

public sealed class SelectionManager : MonoBehaviour
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;

    [SerializeField] private SelectionOptionObjectAreaDetector _selectionOptionObjectAreaDetector;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    
    [Header("Selectors")]
    [SerializeField] private BuildingSelector _buildingSelector;
    [SerializeField] private BuildingUpgradeSelector _buildingUpgradeSelector;
    
    private Queue<SelectionType> _enqeuedSelections = new();
    private SelectionType _currentSelection;
    private bool _selectionOptionsCanBePlaced;
    private bool _selectionIsActive;
    
    public bool SelectionPhaseIsActive => _selectionIsActive;
    public SelectionType SelectionType => _currentSelection;

    public event Action SelectionStarted; 
    public event Action SelectionEnded;
    public event Action SelectionStepStarted;
    public event Action SelectionStepEnded;
    
    private async void Start()
    {
        _selectionOptionObjectAreaDetector.AddedItem += (optionObject) => ResolveCurrentSelection(optionObject).Forget();

        await UniTask.WaitForSeconds(5);
    }

    [Button] public void DEBUGEnqueueBuildingUpgradeSelection() => EnqueueSelection(SelectionType.BuildingUpgrade);
    
    public bool SelectionOptionCanBePlaced(SelectionType type)
    {
        return _selectionOptionsCanBePlaced && type == _currentSelection;
    }
    
    public void EnqueueSelection(SelectionType selectionType) => _enqeuedSelections.Enqueue(selectionType);
    
    public void TryStartQueuedSelection()
    {
        if (_selectionIsActive) return;
        
        StartSelectionPhase();
        
        if (_enqeuedSelections.Count > 0) StartQueuedSelection().Forget();
        else EndSelectionPhase();
    }

    private void StartSelectionPhase()
    {
        _selectionIsActive = true;

        SelectionStarted?.Invoke();
    }
    
    private async UniTask StartQueuedSelection()
    {
        _currentSelection = _enqeuedSelections.Dequeue();
        
        SelectionStepStarted?.Invoke(); 
        
        switch (_currentSelection)
        {
            case SelectionType.Building:
            {
                if (_globalBuildingContainer.GetPlayerBuildings().Count < _globalStatContainer.Get<MaxBuildings>().Value)
                {
                    await _buildingSelector.StartBuildingsSelection();
                } break;
            }
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.StartUpgradeSelection(); break;
            default: throw new NotImplementedException($"Tried to start selection of type {_currentSelection}");
        }
        
        _selectionOptionsCanBePlaced = true;
    }

    private async UniTask ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        optionObject.ApplySelectedEffect();
        
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        _selectionOptionsCanBePlaced = false;
        
        SelectionStepEnded?.Invoke(); 
        
        await EndSelection();

        if (_enqeuedSelections.Count > 0) StartQueuedSelection().Forget();
        else EndSelectionPhase();
    }
    
    private async UniTask EndSelection()
    {
        switch (_currentSelection)
        {
            case SelectionType.BuildingUpgrade:
            {
                AudioSystem.PlaySFX(AudioEnum.sound_general_gameplay_building_upgrade_selection_complete, transform.position);
                await _buildingUpgradeSelector.EndSelection(); 
                break;
            }
            case SelectionType.Building:
            {
                AudioSystem.PlaySFX(AudioEnum.sound_general_gameplay_building_selection_complete, transform.position);
                await UniTask.WaitForSeconds(1f); 
                break;
            }
        }
    }
    
    private void EndSelectionPhase() 
    {
        _selectionIsActive = false;
        
        SelectionEnded?.Invoke();
    }
}