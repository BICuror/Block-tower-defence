using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class BuildingSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;
    [Inject] private DraggableCreator _draggableCreator;
    
    [SerializeField] private SelectionOptionObject _selectionObject;
    [SerializeField] private int _optionsAmount = 3;
    
    public async void StartBuildingsSelection()
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
        
        List<BuildingSelectionOptionData> buildingDatas = datasContainer.GetDatas(_optionsAmount);

        for (int i = 0; i < buildingDatas.Count; i++)
        {
            DraggableObject selectionOptionObject = await _draggableCreator.CreateDraggableOnRandomPosition(_selectionObject.GetComponent<DraggableObject>(), transform.position, 5);
            
            //DraggableObject draggableObject = await _draggableCreator.CreateDraggableOnRandomPosition(buildingDatas[i].GetComponent<DraggableObject>(), transform.position);
            
            
        }
    }
}
