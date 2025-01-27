using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class BuildingSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;

    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private BuildingSelectionOptionObject _selectionObject;
    [SerializeField] private int _optionsAmount = 3;
    
    public async void StartBuildingsSelection()
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
        
        List<BuildingSelectionOptionData> buildingDatas = datasContainer.GetDatas(_optionsAmount);

        for (int i = 0; i < buildingDatas.Count; i++)
        {
            BuildingSelectionOptionObject selectionOptionObject = await _selectionOptionObjectController.CreateSelectionOptionObject(_selectionObject);
            
            selectionOptionObject.SetBuilding(buildingDatas[i].BuildingPrefab);
        }
    }
}