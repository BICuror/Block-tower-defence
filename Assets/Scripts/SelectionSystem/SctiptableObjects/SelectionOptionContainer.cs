using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectionOptionContainer", menuName = "Block Tower Defence/SelectionOptionContainer", order = 0)]

public class SelectionOptionContainer: ScriptableObject
{
    [SerializeField] protected List<SelectionOption> _optionsList = new List<SelectionOption>();

    public SelectionOption[] GetSelectionOptions(int optionsAmount)
    {
        SelectionOption[] selectionOptions = new SelectionOption[optionsAmount];

        int[] randomIndexes = GetRandomOptionsIndexes(optionsAmount);

        for (int i = 0; i < optionsAmount; i++)
        {
            selectionOptions[i] = _optionsList[randomIndexes[i]];
        }

        return selectionOptions;
    }

    private int[] GetRandomOptionsIndexes(int optionsAmount)
    {
        int[] optionsIndexes = new int[optionsAmount];

        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < _optionsList.Count; i++)
        {
            availableIndexes.Add(i);
        }

        for (int i = 0; i < optionsAmount; i++)
        {
            int randomIndex = Random.Range(0, availableIndexes.Count);

            optionsIndexes[i] = availableIndexes[randomIndex];

            availableIndexes.RemoveAt(randomIndex);
        }

        return optionsIndexes;
    }

    public void TryToRemoveSelectionOption(SelectionOption selectionOption)
    {
        if (selectionOption.ShouldBeRemovedAfterTaking)
        {
            _optionsList.Remove(selectionOption);
        }
    }
}
