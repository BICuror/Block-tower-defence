using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Tutorial.Custom
{
    public sealed class CrystalsTutorialStep : ProgressTutorialStep
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveStateMachine _waveStateMachine;
        [Inject] private ItemsContainer _itemsContainer;
        [Inject] private ItemFactory _itemFactory;
        
        private DraggableObject _startWaveItemDraggableObject;
        
        public override async UniTask StartStep()
        {
            await _itemFactory.CreateItems(new Vector3(_islandDataContainer.Data.IslandRadius, 2, _islandDataContainer.Data.IslandRadius), 5, 1, 2);

            _itemsContainer.ItemAdded += OnItemContainerModification;
            _itemsContainer.ItemRemoved += OnItemContainerModification;
            
            UpdateStartWaveItemDraggableState();

            await EnableUI();
        }

        private void OnItemContainerModification(Item _) => UpdateStartWaveItemDraggableState();
        
        private void UpdateStartWaveItemDraggableState()
        {
            SetProgress(_itemsContainer.ContainedItems.Count);
        }

        public override async UniTask EndStep()
        {
            _itemsContainer.ItemAdded -= OnItemContainerModification;
            _itemsContainer.ItemRemoved -= OnItemContainerModification;
            
            _itemsContainer.ContainedItems.ForEach(item => item.GetComponent<DraggableObject>().SetDraggableState(false));
            
            await DisableUI();
        }
    }
}