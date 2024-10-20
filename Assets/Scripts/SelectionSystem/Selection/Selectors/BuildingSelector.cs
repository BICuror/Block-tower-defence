using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private SelectionOptionDataSelector _selectionOptionDataSelector;

    public void StartBuildingsSelection()
    {
        List<BuildingSelectionOptionData> buildingDatas = _selectionOptionDataSelector.SelectBuildingDatas(3);
    }
}
