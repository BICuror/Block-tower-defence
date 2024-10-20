using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SelectionManager : MonoBehaviour
{
    private int _selectionSize = 3;
    private Queue<SelectionType> _enqeuedSelections = new();

    [SerializeField] private BuildingSelector _buildingSelector;

    public void EnqeueSelection(SelectionType type) => _enqeuedSelections.Enqueue(type); 

    private void StartSelection()
    {
        SelectionType type = _enqeuedSelections.Dequeue();

        switch (type)
        {
            case SelectionType.Building: _buildingSelector.StartBuildingsSelection(); break;
            default: break;
        }
    }
}
