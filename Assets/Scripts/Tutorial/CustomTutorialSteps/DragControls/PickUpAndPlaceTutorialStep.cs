using Cysharp.Threading.Tasks;
using GameControls.States;
using GameControls;
using UnityEngine;
using Zenject;
using Combat;

namespace Tutorial.Custom
{
    public sealed class PickUpAndPlaceTutorialStep : ProgressTutorialStep
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private DraggableCreator _draggableCreator;
        [Inject] private GameController _gameControls;
        [SerializeField] private DraggableObject _towerPrefab;
        private CombatEntity _buildingEntity;
        
        public override async UniTask StartStep()
        {
            DraggableObject draggableObject = await _draggableCreator.CreateDraggableOnRandomPosition(_towerPrefab, new Vector3(_islandDataContainer.Data.IslandRadius, 2, _islandDataContainer.Data.IslandRadius), 5);
            _buildingEntity = draggableObject.GetComponent<CombatEntity>();
            
            _buildingEntity.StatContainer.Get<BuildTime>().SetDefault(0f);
            _buildingEntity.ComponentsContainer.Get<EntityCanvas>().gameObject.SetActive(false);
            _buildingEntity.ComponentsContainer.Get<DraggableObject>().Placed += IncreaseProgress; 
            
            _gameControls.EnableState(ControllerState.Dragging);
            
            await EnableUI();
        }

        public override async UniTask EndStep()
        {
            _buildingEntity.ComponentsContainer.Get<DraggableObject>().Placed -= IncreaseProgress; 
            _buildingEntity.ComponentsContainer.Get<EntityCanvas>().gameObject.SetActive(true);
            
            await DisableUI();

            await UniTask.WaitForSeconds(0.5f);
        }
    }
}