using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;
    [Inject] private GlobalStatContainer _globalStatContainer;
    
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private BuildingSelectionOptionObject _selectionObject;
    
    public async UniTask StartBuildingsSelection()
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
        
        List<BuildingEntity> buildingPrefabs = datasContainer.GetRandomPrefabs(_globalStatContainer.Get<SelectionOptionsAmount>().RoundedValue);

        await _selectionOptionObjectController.CreateSelectionOptionObjects(_selectionObject, buildingPrefabs.Count, InitializeSelectionOption);
        
        void InitializeSelectionOption(BuildingSelectionOptionObject selectionOptionObject)
        {
            int prefabIndex = Random.Range(0, buildingPrefabs.Count);
            
            selectionOptionObject.SetBuilding(buildingPrefabs[prefabIndex]);

            buildingPrefabs.RemoveAt(prefabIndex);
        }
    }
}