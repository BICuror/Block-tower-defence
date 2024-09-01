using UnityEngine;
using Zenject;
using WorldGeneration;

public sealed class BuildingsSelectionCrystal : SelectionCrystal
{
    [Inject] private IslandDataContainer _islandDataContainer; 
    public override SelectionOptionContainer GetOptionContainer() => _islandDataContainer.Data.BuildingsSelectionOptionContainer;

    protected override void ApplySelectionOption(SelectionOption selectionOption)
    {
        BuildingSelectionOption option = selectionOption as BuildingSelectionOption;   

        FindObjectOfType<DraggableCreator>().CreateDraggableOnRandomPosition(option.Building, transform.position);
    }
}
