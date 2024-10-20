using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using WorldGeneration;

public class SelectionOptionDataSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;

    public List<BuildingSelectionOptionData> SelectBuildingDatas(int amount)
    {
        BuildingSelectionOptionDataContainer datasContainer = _islandDataHolder.Data.SelectionContainer.BuildingSelectionOptionDataContainer;
    
        return datasContainer.GetDatas(amount);
    }
}
