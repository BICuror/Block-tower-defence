using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public sealed class BuildingSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;
    [Inject] private GlobalStatContainer _globalStatContainer;

    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private BuildingSelectionOptionObject _selectionObject;
    
    public async UniTask StartBuildingsSelection()
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
        
        List<BuildingSelectionOptionData> buildingDatas = datasContainer.GetDatas(_globalStatContainer.Get<SelectionOptionsAmount>().RoundedValue);

        for (int i = 0; i < buildingDatas.Count; i++)
        {
            BuildingSelectionOptionObject selectionOptionObject = await _selectionOptionObjectController.CreateSelectionOptionObject(_selectionObject);
            
            selectionOptionObject.SetBuilding(buildingDatas[i].BuildingPrefab);
        }
    }
}