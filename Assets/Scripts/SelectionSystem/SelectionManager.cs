using UnityEngine;
using UnityEngine.Events;

public sealed class SelectionManager : MonoBehaviour
{
    [SerializeField] private OptionsCreator _optionsCreator;

    [SerializeField] private SelectionOptionsObjectsPositionCalculator _selectionOptionsObjectsPositionCalculator;

    [SerializeField] private SelectionAnimator _selectionAnimator;

    private SelectionOptionObject[] _selectionOptionObjects;
    public UnityEvent<SelectionOption> OptionChoosen;
    private SelectionOptionContainer _selectionOptionsContainer;
    [SerializeField] private int _selectionOptionsAmount = 3;
    public void SetSelectionOptionAmount(int value) => _selectionOptionsAmount += value;
    public int GetSelectionOptionAmount() => _selectionOptionsAmount;

    public void StartSelection(SelectionCrystal crystal)
    {
        SelectionOption[] selectionOptions = crystal.GetOptionContainer().GetSelectionOptions(GetSelectionOptionAmount());

        _selectionOptionObjects = _optionsCreator.CreateOptionObjects(selectionOptions, crystal is BuildingsSelectionCrystal);
    
        for (int i = 0; i < _selectionOptionObjects.Length; i++)
        {
            _selectionOptionObjects[i].Choosen.AddListener(StopSelection);
        }

        _selectionAnimator.StartSelectionAnimation(_selectionOptionsObjectsPositionCalculator.GetPanelsPosition(_selectionOptionObjects.Length), _selectionOptionObjects, crystal);
    }

    private void StopSelection(SelectionOption selectionOption)
    {   
        _selectionAnimator.StopSelectionAnimation(_selectionOptionObjects);

        OptionChoosen.Invoke(selectionOption);
    }
}
