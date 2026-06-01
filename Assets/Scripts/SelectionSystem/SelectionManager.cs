using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using CuroAudio;
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
    
    private List<SelectionSettings> _enqeuedSelections = new();
    private SelectionSettings _currentSelection;
    private bool _selectionOptionsCanBePlaced;
    private bool _continueSelectionOnEnd;
    private bool _selectionIsActive;
    private int _rerollsLeft;
    
    public bool SelectionPhaseIsActive => _selectionIsActive;
    public SelectionSettings CurrentSelection => _currentSelection;
    public bool SelectionOptionsCanBePlaced => _selectionOptionsCanBePlaced;
    public int RerollsLeft => _rerollsLeft;

    public event Action SelectionStarted; 
    public event Action SelectionEnded;
    public event Action SelectionStepStarted;
    public event Action SelectionStepEnded;
    
    private async void Start()
    {
        _selectionOptionObjectAreaDetector.AddedItem += (optionObject) => ResolveCurrentSelection(optionObject).Forget();

        await UniTask.WaitForSeconds(5);
    }

    [Button] public void DEBUGEnqueueBuildingUpgradeSelection() => EnqueueSelection(new SelectionSettings(SelectionType.BuildingUpgrade));
    
    public void EnqueueSelection(SelectionSettings settings) => _enqeuedSelections.Add(settings);
    public void AddRerolls(int amount) => _rerollsLeft += amount;
    
    public void TryStartQueuedSelection()
    {
        if (_selectionIsActive) return;
        
        StartSelectionPhase();
        
        if (_enqeuedSelections.Count > 0) StartQueuedSelection().Forget();
        else EndSelectionPhase();
    }

    public void StartSelectionPhase()
    {
        _selectionIsActive = true;

        SelectionStarted?.Invoke();
    }

    public async UniTask StartSelection(SelectionSettings settings, bool continueIfOtherSelectionsExist = true)
    {
        _continueSelectionOnEnd = continueIfOtherSelectionsExist;
        _currentSelection = settings;
        SelectionStepStarted?.Invoke();

        switch (_currentSelection.SelectionType)
        {
            case SelectionType.Building:
            {
                if (_globalBuildingContainer.GetPlayerBuildings().Count < _globalStatContainer.Get<MaxBuildings>().Value)
                {
                    await _buildingSelector.StartSelection(settings);
                }

                break;
            }
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.StartSelection(settings); break;
            default: throw new NotImplementedException($"Tried to start selection of type {_currentSelection}");
        }

        _selectionOptionsCanBePlaced = true;

        await UniTask.WaitWhile(() => _selectionOptionsCanBePlaced);
    }
    
    private async UniTask StartQueuedSelection(bool continueIfOtherSelectionsExist = true)
    {
        SelectionSettings settings = _enqeuedSelections[0];
        
        _enqeuedSelections.RemoveAt(0);
        
        await StartSelection(settings, continueIfOtherSelectionsExist);
    }

    private async UniTask ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        if (optionObject is RerollSelectionOptionObject)
        {
            RerollCurrentSelection().Forget();
            return;
        }
        
        optionObject.ApplySelectedEffect();
        
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        _selectionOptionsCanBePlaced = false;
        
        SelectionStepEnded?.Invoke(); 
        
        await EndSelection();
        
        if (_enqeuedSelections.Count > 0 && _continueSelectionOnEnd) StartQueuedSelection().Forget();
        else EndSelectionPhase().Forget();
    }
    
    private async UniTask EndSelection()
    {
        switch (_currentSelection.SelectionType)
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
    
    private async UniTask EndSelectionPhase() 
    {
        _selectionIsActive = false;
        
        SelectionEnded?.Invoke();
    }

    public async UniTask RerollCurrentSelection()
    {
        _rerollsLeft--;
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        _selectionOptionsCanBePlaced = false;

        SelectionSettings copiedSelectionSettings = new SelectionSettings(_currentSelection.SelectionType, _currentSelection.SelectionOptionsAmount + 1, _currentSelection.Target);
                    
        SelectionStepEnded?.Invoke(); 
        
        await EndSelection();
        
        StartSelection(copiedSelectionSettings).Forget();
    }
}

public sealed class SelectionSettings
{
    public readonly SelectionType SelectionType;
    public int SelectionOptionsAmount;
    public GameObject Target;

    public SelectionSettings(SelectionType selectionType, int selectionOptionsAmount = 0, GameObject target = null)
    {
        SelectionType = selectionType;
        SelectionOptionsAmount = selectionOptionsAmount;
        Target = target;
    }
}