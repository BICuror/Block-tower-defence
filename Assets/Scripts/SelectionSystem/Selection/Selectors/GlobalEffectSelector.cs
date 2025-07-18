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
        List<ToggleGlobalEffectData> effectDatas = _toggleEffectDataSelectionContainer.GetGlobalEffects(_globalStatContainer.Get<SelectionOptionsAmount>().RoundedValue);

        await _selectionOptionObjectController.CreateSelectionOptionObjects(_selectionObject, effectDatas.Count, InitializeSelectionOption);
        
        void InitializeSelectionOption(GlobalEffectSelectionOptionObject selectionOptionObject)
        {
            int prefabIndex = Random.Range(0, effectDatas.Count);
            
            selectionOptionObject.SetGlobalEffectData(effectDatas[prefabIndex]);

            effectDatas.RemoveAt(prefabIndex);
        }
    }
}