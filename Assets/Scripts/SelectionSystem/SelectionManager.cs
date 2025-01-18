using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class SelectionManager : MonoBehaviour
{
    private Queue<SelectionType> _enqeuedSelections = new();
    
    private SelectionType _currentSelection;

    [SerializeField] private SelectionOptionObjectAreaDetector _selectionOptionObjectAreaDetector;
    [SerializeField] private ItemDetector _itemDetector;
    [SerializeField] private BuildingSelector _buildingSelector;

    private async void Start()
    {
        await UniTask.WaitForSeconds(1);
        
        StartSelection(SelectionType.Building);

        _selectionOptionObjectAreaDetector.AddedItem += ResolveCurrentSelection;
    }

    public void EnqeueSelection(SelectionType type) => _enqeuedSelections.Enqueue(type);

    public void StartQueuedSelection()
    {
        SelectionType type = _enqeuedSelections.Dequeue();

        StartSelection(type);
    }
    
    public void StartSelection(SelectionType type)
    {
        _currentSelection = type;
        switch (_currentSelection)
        {
            case SelectionType.Building: _buildingSelector.StartBuildingsSelection(); break;
            default: break;
        }
    }
    
    public void ResolveCurrentSelection(SelectionOptionObject optionObject)
    {
        
    }

    public bool SelectionOptionCanBePlaced(SelectionType type)
    {
        return type == _currentSelection;
    }
}
