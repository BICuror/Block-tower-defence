using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class GlobalEffectSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;

    [SerializeField] private ToggleEffectDataSelectionContainer _toggleEffectDataSelectionContainer;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private GlobalEffectSelectionOptionObject _selectionObject;
    [SerializeField] private int _optionsAmount = 3;
    
    public async void StartGlobalEffectSelection()
    {
        List<ToggleGlobalEffectData> buildingDatas = _toggleEffectDataSelectionContainer.GetGlobalEffects(_optionsAmount);

        for (int i = 0; i < buildingDatas.Count; i++)
        {
            GlobalEffectSelectionOptionObject selectionOptionObject = await _selectionOptionObjectController.CreateSelectionOptionObject(_selectionObject);
            
            selectionOptionObject.SetGlobalEffectData(buildingDatas[i]);
        }
    }
}