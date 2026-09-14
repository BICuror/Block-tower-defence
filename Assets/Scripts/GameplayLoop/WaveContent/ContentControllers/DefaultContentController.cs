using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public sealed class DefaultContentController : MonoBehaviour
{
    [Inject] private GlobalStatContainer _globalStatContainer;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private WaveIndexContainer _waveIndexContainer;  
    [Inject] private SelectionManager _selectionManager;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private ItemsContainer _itemsContainer;
    [Inject] private ItemFactory _itemFactory; 
    
    private void Awake()
    {
        IdleStateController idleStateController = _waveStateMachine.GetWaveStateController(WaveState.Idle) as IdleStateController;
        
        idleStateController.EnteredStateStarted += TryEnableWaveContent;
        idleStateController.EnteredStateCompleted += GenerateWaveItems;
        idleStateController.OnPreEnemyGroupGeneraton = OnPreEnemyGroupGeneration;
    }

    private async UniTask OnPreEnemyGroupGeneration()
    {
        await TryEnableVictoryScreen();
        
        await TryStartBuildingSelection();
    }

    private async UniTask TryEnableVictoryScreen()
    {
        if (_waveIndexContainer.GetMaxWave() >= _waveIndexContainer.GetCurrentWave()) return;
        
        IdleStateController idleStateController = _waveStateMachine.GetWaveStateController(WaveState.Idle) as IdleStateController;
        
        idleStateController.EnableVictoryScreen();
        
        await UniTask.WaitWhile(() => gameObject);
    }
    
    private async UniTask TryStartBuildingSelection()
    {
        if (_waveIndexContainer.GetCurrentWaveContent().Content.Contains(WaveContentType.BuildingSelection))
        {
            _selectionManager.StartSelectionPhase();
            _selectionManager.StartSelection(new SelectionSettings(SelectionType.Building), false).Forget();
            await UniTask.WaitWhile(() => _selectionManager.SelectionPhaseIsActive);
        }
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

        int additionalItemsToCreate = _globalStatContainer.Get<AdditionalCrystalsAmount>().RoundedValue;

        int additionalItemStrength = additionalItemsToCreate * waveContent.AdditionalItemStrength * additionalItemsToCreate;
        
        _itemFactory.CreateItems(_itemsContainer.transform.position, waveContent.CombinedItemStrength + additionalItemStrength, minimalItemStrength, waveContent.ItemsAmount + additionalItemsToCreate, true).Forget();
    }
}