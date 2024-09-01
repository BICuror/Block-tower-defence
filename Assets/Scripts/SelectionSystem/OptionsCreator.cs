using UnityEngine;

public sealed class OptionsCreator : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private SelectionOptionObject _buildingSelectionObjectPrefab;
    [SerializeField] private SelectionOptionObject _upgradeSelectionObjectPrefab;

    public SelectionOptionObject[] CreateOptionObjects(SelectionOption[] selectionOptions, bool buildings)
    {
        SelectionOptionObject[] selectionOptionsObjects = new SelectionOptionObject[selectionOptions.Length];

        for (int i = 0; i < selectionOptions.Length; i++)
        {
            if (buildings)
            {
                selectionOptionsObjects[i] = Instantiate(_buildingSelectionObjectPrefab, Vector3.zero, Quaternion.identity, _parent);
            }
            else
            {
                selectionOptionsObjects[i] = Instantiate(_upgradeSelectionObjectPrefab, Vector3.zero, Quaternion.identity, _parent);
            }
            
            selectionOptionsObjects[i].transform.localRotation = Quaternion.Euler(90f, -90f, 90f);
            selectionOptionsObjects[i].transform.localPosition = Vector3.zero;
            selectionOptionsObjects[i].transform.localScale = Vector3.zero;

            selectionOptionsObjects[i].SetSelectionOption(selectionOptions[i]);
        }

        return selectionOptionsObjects;
    }
}
