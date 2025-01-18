using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class SelectionOptionDataSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;

    public List<BuildingSelectionOptionData> SelectBuildingDatas(int amount)
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
    
        return datasContainer.GetDatas(amount);
    }
}