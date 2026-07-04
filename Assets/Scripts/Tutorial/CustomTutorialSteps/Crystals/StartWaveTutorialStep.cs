using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class StartWaveTutorialStep : UITutorialStep
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveStateMachine _waveStateMachine;
        [Inject] private ItemsContainer _itemsContainer;
        [Inject] private ItemFactory _itemFactory;
        
        public override async UniTask StartStep()
        {
            List<Vector2Int> spawnPosition = TileMap.GetSuitablePositionsInRadius(_ => true, new Vector2Int(_islandDataContainer.Data.IslandRadius, _islandDataContainer.Data.IslandRadius));
            Vector2Int selectedPosition = spawnPosition[Random.Range(0, spawnPosition.Count)];
            
            await _itemFactory.CreateStartWaveItem(new Vector3(_islandDataContainer.Data.IslandRadius, 2, _islandDataContainer.Data.IslandRadius), selectedPosition);

            await EnableUI();

            await UniTask.WaitUntil(() => _waveStateMachine.CurrentState == WaveState.Attack);
            
            CompleteStep();
        }
        
        public override async UniTask EndStep()
        {
            await DisableUI();
        }
    }
}