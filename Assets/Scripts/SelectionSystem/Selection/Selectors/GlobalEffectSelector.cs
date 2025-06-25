using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public sealed class GlobalEffectSelector : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataHolder;
    [Inject] private GlobalStatContainer _globalStatContainer;

    [SerializeField] private ToggleEffectDataSelectionContainer _toggleEffectDataSelectionContainer;
    [SerializeField] private SelectionOptionObjectController _selectionOptionObjectController;
    [SerializeField] private GlobalEffectSelectionOptionObject _selectionObject;
    
    public async UniTask StartGlobalEffectSelection()
    {
        List<ToggleGlobalEffectData> buildingDatas = _toggleEffectDataSelectionContainer.GetGlobalEffects(_globalStatContainer.Get<SelectionOptionsAmount>().RoundedValue);

        for (int i = 0; i < buildingDatas.Count; i++)
        {
            GlobalEffectSelectionOptionObject selectionOptionObject = await _selectionOptionObjectController.CreateSelectionOptionObject(_selectionObject);
            
            selectionOptionObject.SetGlobalEffectData(buildingDatas[i]);
        }
    }
}