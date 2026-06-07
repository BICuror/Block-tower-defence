using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public sealed class WaveContentController : MonoBehaviour
{
    [SerializeField] private IdleStateController _idleStateController;
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private WaveIndexContainer _waveIndexContainer;  
    [Inject] private SelectionManager _selectionManager;
    [Inject] private ItemFactory _itemFactory; 
    
    [SerializeField] private Transform _townhallTransform;
    
    private void Awake()
    {
        _idleStateController.EnteredStateStarted += TryEnableWaveContent;
        _idleStateController.EnteredStateCompleted += GenerateWaveItems;
    }

    private void TryEnableWaveContent()
    {
        WaveContent waveContent = _waveIndexContainer.GetCurrentWaveContent();
        
        waveContent.Content.ForEach(EnableContent);
    }

    private void EnableContent(WaveContentType contentType)
    {
        switch (contentType)
        {
            case WaveContentType.BuildingSelection: break;
            case WaveContentType.FreeBuildingUpgradeSelection: _selectionManager.EnqueueSelection(new SelectionSettings(SelectionType.BuildingUpgrade, 4)); break;
            case WaveContentType.BossWave: break;
        }
    }

    private void GenerateWaveItems()
    {
        WaveContent waveContent = _waveIndexContainer.GetCurrentWaveContent();
        int minimalItemStrength = _islandDataContainer.Data.WavesContentConfig.MinimalItemStrength;

        _itemFactory.CreateItems(_townhallTransform.position, waveContent.CombinedItemStrength, minimalItemStrength, waveContent.ItemsAmount).Forget();

        int additionalItemsToCreate = _globalStatContainer.Get<AdditionalCrystalsAmount>().RoundedValue;
        
        if (additionalItemsToCreate > 0)
        {
            _itemFactory.CreateItems(_townhallTransform.position, additionalItemsToCreate * waveContent.AdditionalItemStrength, 0, additionalItemsToCreate).Forget();
        }
    }
}