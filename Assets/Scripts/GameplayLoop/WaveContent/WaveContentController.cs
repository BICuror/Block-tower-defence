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
        _selectionManager.SelectionEnded += GenerateWaveItems;
    }

    private void TryEnableWaveContent()
    {
        WaveContent waveContent = GetCurrentWaveContent();
        
        waveContent.Content.ForEach(EnableContent);
    }

    private void EnableContent(WaveContentType contentType)
    {
        switch (contentType)
        {
            case WaveContentType.BuildingSelection: _selectionManager.EnqueueSelection(SelectionType.Building); break;
            case WaveContentType.FreeBuildingUpgradeSelection: _selectionManager.EnqueueSelection(SelectionType.BuildingUpgrade); break;
            case WaveContentType.BossWave: break;
        }
    }

    private void GenerateWaveItems()
    {
        WaveContent waveContent = GetCurrentWaveContent();

        _itemFactory.CreateStartWaveItem(_townhallTransform.transform.position).Forget();
        _itemFactory.CreateItems(_townhallTransform.position, waveContent.CombinedItemStrength, waveContent.MinimalItemStrength, waveContent.ItemsAmount).Forget();

        int additionalItemsToCreate = _globalStatContainer.Get<AdditionalCrystalsAmount>().RoundedValue;
        
        if (additionalItemsToCreate > 0)
        {
            _itemFactory.CreateItems(_townhallTransform.position, additionalItemsToCreate * waveContent.AdditionalItemStrength, 0, additionalItemsToCreate).Forget();
        }
    }

    private WaveContent GetCurrentWaveContent()
    {
        return _islandDataContainer.Data.WavesContentConfig.GetWaveContent(_waveIndexContainer.GetCurrentWave());
    }
}