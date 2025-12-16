using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using System;
using TMPro;

public sealed class SelectionManager : MonoBehaviour
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    private Queue<SelectionSettings> _enqeuedSelections = new();
    private SelectionSettings _currentSelectionSettings;

    [SerializeField] private SelectionOptionObjectAreaDetector _selectionOptionObjectAreaDetector;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    
    [Header("Indicators")]
    [SerializeField] private List<SelectionIndicatorContainers> _selectionIndicatorContainers;
    
    [Header("Selectors")]
    [SerializeField] private BuildingSelector _buildingSelector;
    [SerializeField] private BuildingUpgradeSelector _buildingUpgradeSelector;
    private bool _selectionOptionsCanBePlaced;
    private bool _selectionIsActive;
    
    public bool SelectionPhaseIsActive => _selectionIsActive;
    
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
            StartQueuedSelection().Forget();
        }
    }
    
    private async UniTask StartQueuedSelection()
    {
        _selectionIsActive = true;
        _currentSelectionSettings = _enqeuedSelections.Dequeue();
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

        EnableSelectionIndicator(_currentSelectionSettings.Type);
        
        _selectionOptionsCanBePlaced = true;
    }

    private async UniTask ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        optionObject.ApplyEffect();
        
        _selectionOptionObjectController.DestroyAllCreatedSelectionOptions();
        _selectionOptionsCanBePlaced = false;
        
        await EndSelection(_currentSelectionSettings);

        if (_enqeuedSelections.Count > 0) StartQueuedSelection().Forget();
        else _selectionIsActive = false;
    }
    
    private async UniTask EndSelection(SelectionSettings selectionSettings)
    {
        DisableSelectionIndicator(selectionSettings.Type).Forget();
        
        _selectionIsActive = false;
        
        switch (_currentSelectionSettings.Type)
        {
            case SelectionType.BuildingUpgrade: await _buildingUpgradeSelector.EndSelection(); break;
            case SelectionType.Building: await UniTask.WaitForSeconds(1f); break;
        }
    }

    private async UniTask DisableSelectionIndicator(SelectionType type)
    {
        TextMeshPro indicator = _selectionIndicatorContainers.Find(container => container.Type == type).Indicator;
        await indicator.DOFade(0f, 1f).From(1f).AsyncWaitForCompletion();
        indicator.gameObject.SetActive(false);
    }

    private void EnableSelectionIndicator(SelectionType type)
    {
        TextMeshPro indicator = _selectionIndicatorContainers.Find(container => container.Type == type).Indicator;
        indicator.gameObject.SetActive(true);
        indicator.DOFade(1f, 1f).From(0f);
    }

    [Serializable] private sealed class SelectionIndicatorContainers
    {
        [SerializeField] private SelectionType _selectionType;
        [SerializeField] private TextMeshPro _selectionIndicator;
        
        public SelectionType Type => _selectionType;
        public TextMeshPro Indicator => _selectionIndicator;
    }
}