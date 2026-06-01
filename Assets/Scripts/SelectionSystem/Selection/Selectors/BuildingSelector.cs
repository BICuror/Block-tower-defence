using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingSelector : SelectorBase<BuildingSelectionOptionObject>
{
    [Inject] private IslandDataContainer _islandDataHolder;
    private List<BuildingEntity> _selectedBuildingEntities = new();
    
    public override async UniTask StartSelection(SelectionSettings settings)
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
        
        _selectedBuildingEntities = datasContainer.GetRandomPrefabs(GetSelectionOptionsAmount(settings));

        await CreateOptionObjects(settings);
    }
    
    protected override void InitializeSelectionOption(BuildingSelectionOptionObject optionObject)
    {
        int prefabIndex = Random.Range(0, _selectedBuildingEntities.Count);
            
        optionObject.SetBuilding(_selectedBuildingEntities[prefabIndex]);

        _selectedBuildingEntities.RemoveAt(prefabIndex);
    }
}